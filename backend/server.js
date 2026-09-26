require("dotenv").config();
const express = require("express");
const cors = require("cors");
const { MongoClient } = require("mongodb");
const jwt = require("jsonwebtoken");

const app = express();
const PORT = process.env.PORT || 3000;

if (!process.env.JWT_SECRET) {
  console.error("FATAL: JWT_SECRET env var is required");
  process.exit(1);
}
const JWT_SECRET = process.env.JWT_SECRET;
const JWT_EXPIRY = "12h";

// Legitimate maxima per sub-score, set from observed gameplay. The monotonic
// $max update below does the heavy lifting; the clamp only exists to block
// INT_MAX-style garbage.
const MAX_SCORES = {
  fishSelectionScore: 500,
  fishPrepScore:      4,
  fishCheckTempScore: 400,
  fishPackagingScore: 3,
  stageCount:         11,
};

// Employee ID rule — must stay identical to the Unity client's. There is no
// roster: the organisation enforces correct IDs out-of-band, so any
// well-formed ID is a player. Uppercasing makes "ab12" and "AB12" one player
// instead of two leaderboard rows. Returns null when invalid.
const EMPLOYEE_ID_RE = /^[A-Z0-9-]{1,50}$/;
function normalizeEmployeeId(raw) {
  if (typeof raw !== "string") return null;
  const id = raw.trim().toUpperCase();
  return EMPLOYEE_ID_RE.test(id) ? id : null;
}

app.use(cors());
app.use(express.json());

// ── MongoDB connection ─────────────────────────────────────
// Collection:
//   gameData — one doc per player, keyed by personCode (the normalized
//              employee ID). Created by the player's first score save.
const MONGODB_URI      = process.env.MONGODB_URI || "mongodb://localhost:27017";
const MONGODB_DATABASE = process.env.MONGODB_DATABASE || "food";

// Leaderboard order: highest total first, older score wins ties, personCode
// makes it total so ranks are stable. Backed by the compound index below.
const LB_SORT     = { totalScore: -1, lastUpdated: 1, personCode: 1 };
const LB_SORT_REV = { totalScore: 1, lastUpdated: -1, personCode: -1 };

// Lazy + cached, like the old SQL pool: connect and ensure indexes once per
// process. Any failure resets the cache so the next request retries.
let dbPromise = null;
function getDb() {
  if (!dbPromise) {
    const client = new MongoClient(MONGODB_URI);
    dbPromise = client.connect()
      .then(async () => {
        const db = client.db(MONGODB_DATABASE);
        await Promise.all([
          db.collection("gameData").createIndex({ personCode: 1 }, { unique: true }),
          db.collection("gameData").createIndex(LB_SORT),
        ]);
        console.log("Connected to MongoDB");
        return db;
      })
      .catch(async (err) => {
        dbPromise = null;
        await client.close().catch(() => {});
        throw err;
      });
  }
  return dbPromise;
}

// Zeros when the player has no gameData doc yet (first login).
function mapGameData(doc) {
  return {
    fishSelectionScore: doc?.fishSelectionScore ?? 0,
    fishPrepScore:      doc?.fishPrepScore ?? 0,
    fishCheckTempScore: doc?.fishCheckTempScore ?? 0,
    fishPackagingScore: doc?.fishPackagingScore ?? 0,
    stageCount:         doc?.stageCount ?? 0,
    totalScore:         doc?.totalScore ?? 0,
  };
}

function clamp(value, max) {
  const n = Number.isFinite(value) ? Math.floor(value) : 0;
  if (n < 0) return 0;
  if (n > max) return max;
  return n;
}

// gameData docs strictly ahead of `gd` in LB_SORT order.
function aheadOf(gd) {
  return {
    $or: [
      { totalScore: { $gt: gd.totalScore } },
      { totalScore: gd.totalScore, lastUpdated: { $lt: gd.lastUpdated } },
      { totalScore: gd.totalScore, lastUpdated: gd.lastUpdated, personCode: { $lt: gd.personCode } },
    ],
  };
}

// ── Auth middleware ────────────────────────────────────────
function requireAuth(req, res, next) {
  const header = req.headers.authorization || "";
  const match = header.match(/^Bearer\s+(.+)$/i);
  if (!match) {
    return res.status(401).json({ error: "Missing or malformed Authorization header" });
  }
  let payload;
  try {
    payload = jwt.verify(match[1], JWT_SECRET);
  } catch (err) {
    return res.status(401).json({ error: "Invalid or expired token" });
  }
  // The token's personCode is the upsert key, so it must already be in
  // normalized form; tokens from older builds without one are rejected.
  if (typeof payload.personCode !== "string" || !EMPLOYEE_ID_RE.test(payload.personCode)) {
    return res.status(401).json({ error: "Token has no valid personCode" });
  }
  req.auth = { personCode: payload.personCode };
  next();
}

// ── Routes ─────────────────────────────────────────────────

app.get("/", (req, res) => {
  res.json({ status: "ok", message: "Food Score API is running" });
});

// Login. Any well-formed employee ID is accepted (400 otherwise) and gets a
// JWT bound to its normalized personCode — all score writes must present it.
app.get("/api/person/:code", async (req, res) => {
  const personCode = normalizeEmployeeId(req.params.code);
  if (!personCode) {
    return res.status(400).json({ success: false, exists: false, error: "Invalid Employee ID" });
  }
  try {
    const db = await getDb();
    // Read-only on purpose: the record is created by the first score save, so
    // a mistyped ID never leaves a ghost row on the leaderboard.
    const gd = await db.collection("gameData").findOne({ personCode });
    const token = jwt.sign({ personCode }, JWT_SECRET, { expiresIn: JWT_EXPIRY });

    res.json({
      success: true,
      exists: !!gd,
      person:   { personCode },
      gameData: mapGameData(gd),
      token,
    });
  } catch (err) {
    console.error("Error fetching person:", err);
    res.status(500).json({ error: "Failed to fetch person", detail: err.message });
  }
});

// Upsert game data. personCode comes from the verified JWT, NOT the body —
// any client-supplied personCode is ignored. Sub-scores are clamped to
// MAX_SCORES and only ever go up (monotonic).
app.post("/api/scores", requireAuth, async (req, res) => {
  try {
    const clamped = {
      fishSelectionScore: clamp(req.body.fishSelectionScore, MAX_SCORES.fishSelectionScore),
      fishPrepScore:      clamp(req.body.fishPrepScore,      MAX_SCORES.fishPrepScore),
      fishCheckTempScore: clamp(req.body.fishCheckTempScore, MAX_SCORES.fishCheckTempScore),
      fishPackagingScore: clamp(req.body.fishPackagingScore, MAX_SCORES.fishPackagingScore),
      stageCount:         clamp(req.body.stageCount,         MAX_SCORES.stageCount),
    };

    // $max keeps each field monotonic ($ifNull seeds a fresh upsert with 0).
    // Mongo has no computed columns, so totalScore is stored and recomputed in
    // the same atomic pipeline update — it can never drift from the sub-scores.
    const bump = { lastUpdated: "$$NOW" };
    for (const [field, value] of Object.entries(clamped)) {
      bump[field] = { $max: [{ $ifNull: ["$" + field, 0] }, value] };
    }

    const db = await getDb();
    // upsert: the player's first save creates their record.
    const doc = await db.collection("gameData").findOneAndUpdate(
      { personCode: req.auth.personCode },
      [
        { $set: bump },
        { $set: { totalScore: { $add: [
          "$fishSelectionScore", "$fishPrepScore", "$fishCheckTempScore", "$fishPackagingScore",
        ] } } },
      ],
      { upsert: true, returnDocument: "after" }
    );

    res.json({ success: true, data: mapGameData(doc) });
  } catch (err) {
    console.error("Error saving score:", err);
    res.status(500).json({ error: "Failed to save score", detail: err.message });
  }
});

// Leaderboard: top 3 + caller's rank + the player one rank above.
app.get("/api/scores", async (req, res) => {
  try {
    // normalizeEmployeeId only accepts a plain string: Express parses
    // ?personCode[$ne]=x into an object, which would otherwise reach findOne
    // as a query operator. Invalid IDs are treated as absent.
    const personCode = normalizeEmployeeId(req.query.personCode);
    const gameData = (await getDb()).collection("gameData");

    // Shape matches LeaderBoardManager's ScoreEntry (JsonUtility is case-sensitive).
    const data = await gameData
      .find({}, { projection: { _id: 0, personCode: 1, totalScore: 1, lastUpdated: 1 } })
      .sort(LB_SORT)
      .limit(3)
      .toArray();
    const result = { success: true, data };

    const gd = personCode && await gameData.findOne({ personCode });
    if (gd) {
      const rank = 1 + await gameData.countDocuments(aheadOf(gd));
      result.player = { rank, personCode, totalScore: gd.totalScore };

      if (rank > 1) {
        // Nearest doc ahead = first of the "ahead" set in reverse order.
        const above = await gameData.findOne(aheadOf(gd), { sort: LB_SORT_REV });
        if (above) {
          result.nextRank = { rank: rank - 1, personCode: above.personCode, totalScore: above.totalScore };
        }
      }
    }

    res.json(result);
  } catch (err) {
    console.error("Error fetching scores:", err);
    res.status(500).json({ error: "Failed to fetch scores", detail: err.message });
  }
});

// ── Start Server (local dev) / Export (Vercel) ─────────────
if (process.env.VERCEL) {
  module.exports = app;
} else {
  app.listen(PORT, () => {
    console.log("Server running on http://localhost:" + PORT);
  });
}
