require("dotenv").config();
const express = require("express");
const cors = require("cors");
const sql = require("mssql");
const jwt = require("jsonwebtoken");

const app = express();
const PORT = process.env.PORT || 3000;

if (!process.env.JWT_SECRET) {
  console.error("FATAL: JWT_SECRET env var is required");
  process.exit(1);
}
const JWT_SECRET = process.env.JWT_SECRET;
const JWT_EXPIRY = "12h";

// Legitimate maxima per sub-score. Loose v1 values — tune down after
// observing real gameplay. The monotonic MERGE below does the heavy
// lifting; the clamp only exists to block INT_MAX-style garbage.
const MAX_SCORES = {
  fishSelectionScore: 1000,
  fishPrepScore:      1000,
  fishCheckTempScore: 1000,
  fishPackagingScore: 1000,
  stageCount:         11,
};

app.use(cors());
app.use(express.json());

// ── SQL Server connection pool ─────────────────────────────
const dbConfig = {
  user: process.env.MSSQL_USER || "sa",
  password: process.env.MSSQL_PASSWORD,
  server: process.env.MSSQL_HOST || "localhost",
  port: parseInt(process.env.MSSQL_PORT || "1433", 10),
  database: process.env.MSSQL_DATABASE || "HRIS",
  options: {
    encrypt: process.env.MSSQL_ENCRYPT === "true",
    trustServerCertificate: true,
  },
  pool: { max: 10, min: 0, idleTimeoutMillis: 30000 },
};

let poolPromise = null;
function getPool() {
  if (!poolPromise) {
    poolPromise = sql.connect(dbConfig)
      .then((pool) => { console.log("Connected to SQL Server"); return pool; })
      .catch((err) => { poolPromise = null; throw err; });
  }
  return poolPromise;
}

// Trimmed projection of PersonDetail — the full row is heavy (image column,
// dozens of unused fields). Add columns here when the game needs more of them.
const PERSON_FIELDS = `
  pd.PersonID, pd.PersonCode, pd.FnameT, pd.LnameT, pd.FnameE, pd.LnameE,
  pd.NickName, pd.PositionNameT, pd.Company_NameT
`;

function mapPerson(row) {
  return {
    personID:      row.PersonID,
    personCode:    row.PersonCode,
    fnameT:        row.FnameT,
    lnameT:        row.LnameT,
    fnameE:        row.FnameE,
    lnameE:        row.LnameE,
    nickName:      row.NickName,
    positionNameT: row.PositionNameT,
    companyNameT:  row.Company_NameT,
  };
}

function mapGameData(row) {
  return {
    fishSelectionScore: row.FishSelectionScore ?? 0,
    fishPrepScore:      row.FishPrepScore ?? 0,
    fishCheckTempScore: row.FishCheckTempScore ?? 0,
    fishPackagingScore: row.FishPackagingScore ?? 0,
    stageCount:         row.StageCount ?? 0,
    totalScore:         row.TotalScore ?? 0,
  };
}

function clamp(value, max) {
  const n = Number.isFinite(value) ? Math.floor(value) : 0;
  if (n < 0) return 0;
  if (n > max) return max;
  return n;
}

// ── Auth middleware ────────────────────────────────────────
function requireAuth(req, res, next) {
  const header = req.headers.authorization || "";
  const match = header.match(/^Bearer\s+(.+)$/i);
  if (!match) {
    return res.status(401).json({ error: "Missing or malformed Authorization header" });
  }
  try {
    const payload = jwt.verify(match[1], JWT_SECRET);
    req.auth = { personID: payload.personID, personCode: payload.personCode };
    next();
  } catch (err) {
    return res.status(401).json({ error: "Invalid or expired token" });
  }
}

// ── Routes ─────────────────────────────────────────────────

app.get("/", (req, res) => {
  res.json({ status: "ok", message: "Food Score API is running" });
});

// Login. Lookup by PersonCode against the HRIS roster. Returns 404 if the
// code is not in the roster so the client can reject the login. Game data
// joins via PersonID. On success, issues a JWT bound to PersonID — all
// score writes must present this token.
app.get("/api/person/:code", async (req, res) => {
  try {
    const pool = await getPool();
    const result = await pool.request()
      .input("code", sql.VarChar(50), req.params.code)
      .query(`
        SELECT ${PERSON_FIELDS},
               gd.FishSelectionScore, gd.FishPrepScore, gd.FishCheckTempScore,
               gd.FishPackagingScore, gd.StageCount, gd.TotalScore
        FROM dbo.PersonDetail pd
        LEFT JOIN dbo.PersonGameData gd ON gd.PersonID = pd.PersonID
        WHERE pd.PersonCode = @code
      `);

    if (result.recordset.length === 0) {
      return res.status(404).json({
        success: false,
        exists: false,
        error: "PersonCode not found in HRIS roster",
      });
    }

    const row = result.recordset[0];
    const token = jwt.sign(
      { personID: row.PersonID, personCode: row.PersonCode },
      JWT_SECRET,
      { expiresIn: JWT_EXPIRY }
    );

    res.json({
      success: true,
      exists: true,
      person:   mapPerson(row),
      gameData: mapGameData(row),
      token,
    });
  } catch (err) {
    console.error("Error fetching person:", err);
    res.status(500).json({ error: "Failed to fetch person", detail: err.message });
  }
});

// Upsert game data. PersonID comes from the verified JWT, NOT the body —
// any client-supplied personCode is ignored. Sub-scores are clamped to
// MAX_SCORES, and the MERGE only overwrites a column when the new value
// is greater than the stored one (monotonic).
app.post("/api/scores", requireAuth, async (req, res) => {
  try {
    const sel   = clamp(req.body.fishSelectionScore, MAX_SCORES.fishSelectionScore);
    const prep  = clamp(req.body.fishPrepScore,      MAX_SCORES.fishPrepScore);
    const temp  = clamp(req.body.fishCheckTempScore, MAX_SCORES.fishCheckTempScore);
    const pack  = clamp(req.body.fishPackagingScore, MAX_SCORES.fishPackagingScore);
    const stage = clamp(req.body.stageCount,         MAX_SCORES.stageCount);

    const pool = await getPool();

    await pool.request()
      .input("pid",   sql.Numeric(18, 0), req.auth.personID)
      .input("sel",   sql.Int, sel)
      .input("prep",  sql.Int, prep)
      .input("temp",  sql.Int, temp)
      .input("pack",  sql.Int, pack)
      .input("stage", sql.Int, stage)
      .query(`
        MERGE dbo.PersonGameData AS target
        USING (SELECT @pid AS PersonID) AS source
          ON target.PersonID = source.PersonID
        WHEN MATCHED THEN UPDATE SET
          FishSelectionScore = CASE WHEN @sel   > FishSelectionScore THEN @sel   ELSE FishSelectionScore END,
          FishPrepScore      = CASE WHEN @prep  > FishPrepScore      THEN @prep  ELSE FishPrepScore      END,
          FishCheckTempScore = CASE WHEN @temp  > FishCheckTempScore THEN @temp  ELSE FishCheckTempScore END,
          FishPackagingScore = CASE WHEN @pack  > FishPackagingScore THEN @pack  ELSE FishPackagingScore END,
          StageCount         = CASE WHEN @stage > StageCount         THEN @stage ELSE StageCount         END,
          LastUpdated        = GETDATE()
        WHEN NOT MATCHED THEN
          INSERT (PersonID, FishSelectionScore, FishPrepScore, FishCheckTempScore, FishPackagingScore, StageCount)
          VALUES (@pid, @sel, @prep, @temp, @pack, @stage);
      `);

    const fetched = await pool.request()
      .input("pid", sql.Numeric(18, 0), req.auth.personID)
      .query(`
        SELECT FishSelectionScore, FishPrepScore, FishCheckTempScore,
               FishPackagingScore, StageCount, TotalScore, LastUpdated
        FROM dbo.PersonGameData
        WHERE PersonID = @pid
      `);

    res.json({ success: true, data: mapGameData(fetched.recordset[0]) });
  } catch (err) {
    console.error("Error saving score:", err);
    res.status(500).json({ error: "Failed to save score", detail: err.message });
  }
});

// Leaderboard: top 3 + caller's rank + the player one rank above.
app.get("/api/scores", async (req, res) => {
  try {
    const personCode = req.query.personCode;
    const pool = await getPool();

    const top3 = await pool.request().query(`
      SELECT TOP 3
             pd.PersonCode, gd.TotalScore, gd.LastUpdated,
             pd.FnameE, pd.LnameE, pd.NickName
      FROM dbo.PersonGameData gd
      JOIN dbo.PersonDetail   pd ON pd.PersonID = gd.PersonID
      ORDER BY gd.TotalScore DESC, gd.LastUpdated ASC
    `);

    const result = { success: true, data: top3.recordset };

    if (personCode) {
      const ranks = await pool.request()
        .input("code", sql.VarChar(50), personCode)
        .query(`
          WITH ranked AS (
            SELECT pd.PersonCode, gd.TotalScore,
                   ROW_NUMBER() OVER (ORDER BY gd.TotalScore DESC, gd.LastUpdated ASC) AS rnk,
                   pd.FnameE, pd.LnameE, pd.NickName
            FROM dbo.PersonGameData gd
            JOIN dbo.PersonDetail   pd ON pd.PersonID = gd.PersonID
          )
          SELECT * FROM ranked
          WHERE rnk IN (
            SELECT rnk     FROM ranked WHERE PersonCode = @code
            UNION
            SELECT rnk - 1 FROM ranked WHERE PersonCode = @code
          )
          ORDER BY rnk;
        `);

      const rows  = ranks.recordset;
      const me    = rows.find((r) => r.PersonCode === personCode);
      const above = me ? rows.find((r) => r.rnk === me.rnk - 1) : null;

      if (me)    result.player   = { rank: me.rnk,    personCode: me.PersonCode,    totalScore: me.TotalScore,    fnameE: me.FnameE,    lnameE: me.LnameE,    nickName: me.NickName };
      if (above) result.nextRank = { rank: above.rnk, personCode: above.PersonCode, totalScore: above.TotalScore, fnameE: above.FnameE, lnameE: above.LnameE, nickName: above.NickName };
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
