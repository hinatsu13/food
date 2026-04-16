require("dotenv").config();
const express = require("express");
const cors = require("cors");
const mongoose = require("mongoose");

const app = express();
const PORT = process.env.PORT || 3000;

// ── Middleware ──────────────────────────────────────────────
app.use(cors());
app.use(express.json());

// ── MongoDB Connection ─────────────────────────────────────
mongoose
  .connect(process.env.MONGODB_URI)
  .then(() => console.log("Connected to MongoDB"))
  .catch((err) => console.error("MongoDB connection error:", err));

// ── Score Schema ───────────────────────────────────────────
const scoreSchema = new mongoose.Schema(
  {
    playerName:        { type: String, required: true, unique: true },
    fishSelectionScore:{ type: Number, default: 0 },
    fishPrepScore:     { type: Number, default: 0 },
    fishCheckTempScore:{ type: Number, default: 0 },
    fishPackagingScore:{ type: Number, default: 0 },
    totalScore:        { type: Number, default: 0 },
  },
  { timestamps: true }
);

const Score = mongoose.model("Score", scoreSchema);

// ── Routes ─────────────────────────────────────────────────

// Health check
app.get("/", (req, res) => {
  res.json({ status: "ok", message: "Food Score API is running" });
});

// Save or update a score (upsert by playerName)
app.post("/api/scores", async (req, res) => {
  try {
    const {
      playerName,
      fishSelectionScore,
      fishPrepScore,
      fishCheckTempScore,
      fishPackagingScore,
    } = req.body;

    if (!playerName) {
      return res.status(400).json({ error: "playerName is required" });
    }

    const totalScore =
      (fishSelectionScore || 0) +
      (fishPrepScore || 0) +
      (fishCheckTempScore || 0) +
      (fishPackagingScore || 0);

    // Update if player exists, create if not
    const score = await Score.findOneAndUpdate(
      { playerName },
      {
        playerName,
        fishSelectionScore: fishSelectionScore || 0,
        fishPrepScore: fishPrepScore || 0,
        fishCheckTempScore: fishCheckTempScore || 0,
        fishPackagingScore: fishPackagingScore || 0,
        totalScore,
      },
      { upsert: true, new: true, setDefaultsOnInsert: true }
    );

    res.status(200).json({ success: true, data: score });
  } catch (err) {
    console.error("Error saving score:", err);
    res.status(500).json({ error: "Failed to save score" });
  }
});

// Get a player's scores by name
app.get("/api/player/:name", async (req, res) => {
  try {
    const player = await Score.findOne({ playerName: req.params.name }).select("-__v");

    if (!player) {
      return res.json({ success: true, exists: false });
    }

    res.json({ success: true, exists: true, data: player });
  } catch (err) {
    console.error("Error fetching player:", err);
    res.status(500).json({ error: "Failed to fetch player" });
  }
});

// Get leaderboard: top 3 + current player rank + person above
// Query: ?playerName=xxx
app.get("/api/scores", async (req, res) => {
  try {
    const playerName = req.query.playerName;

    // Top 3 only
    const top3 = await Score.find()
      .sort({ totalScore: -1, createdAt: 1 })
      .limit(3)
      .select("-__v");

    const result = {
      success: true,
      data: top3,
    };

    // Find current player's rank + person above
    if (playerName) {
      const allSorted = await Score.find()
        .sort({ totalScore: -1, createdAt: 1 })
        .select("playerName totalScore -_id")
        .lean();

      const playerIndex = allSorted.findIndex(
        (s) => s.playerName === playerName
      );

      if (playerIndex >= 0) {
        result.player = {
          rank: playerIndex + 1,
          playerName: allSorted[playerIndex].playerName,
          totalScore: allSorted[playerIndex].totalScore,
        };

        if (playerIndex > 0) {
          result.nextRank = {
            rank: playerIndex,
            playerName: allSorted[playerIndex - 1].playerName,
            totalScore: allSorted[playerIndex - 1].totalScore,
          };
        }
      }
    }

    res.json(result);
  } catch (err) {
    console.error("Error fetching scores:", err);
    res.status(500).json({ error: "Failed to fetch scores" });
  }
});

// ── Start Server (local dev) / Export (Vercel) ─────────────
if (process.env.VERCEL) {
  // Vercel serverless — just export the app
  module.exports = app;
} else {
  app.listen(PORT, () => {
    console.log("Server running on http://localhost:" + PORT);
  });
}
