# PatayaFood — Canned Fish Production Edutainment Game

An educational Unity WebGL game produced for **Patayafood Company** that walks players through the eleven stages of canned-fish manufacturing. Each stage is a self-contained mini-game — from selecting fresh tuna off a conveyor belt to sealing and sterilizing the finished can — letting players learn the real-world quality and food-safety steps behind every can on the shelf.

Four of the stages are scored. Per-player progress and high scores are persisted to a MongoDB backend so players can resume their factory shift and compete on a global leaderboard.

---

## Key Features

- **11 themed mini-game scenes** mirroring the real Patayafood production line: information briefing, fish selection, hygiene dressing, thawing, prep/gutting, steaming, temperature checks, cleanroom dressing, trimming, packaging, and sterilization.
- **Four scored sub-games** (Selection, Prep, Check-Temp, Packaging) combined into a single `totalScore`.
- **Stage-gated progression** — locked stages in the hub scene unlock as players advance, driven by `StateManager.StageCount`.
- **Persistent player profiles** via a Node/Express + MongoDB REST API. Re-entering a name resumes that player's progress.
- **Global leaderboard** showing the top 3 plus the player's own rank and the player one rank above.
- **Reusable shared plumbing** for loading overlays, star ratings, scene unlocking, and network calls.
- **Vercel-ready backend** — the same Express app runs locally with `npm start` or as a serverless function on Vercel.
- **Editor automation** via the Coplay `unity-mcp` package for MCP-driven Unity workflows.

---

## Prerequisites

| Tool | Version | Purpose |
| --- | --- | --- |
| Unity Editor | **6000.3.5f2** (exact) | Open and build the client |
| Unity Hub | latest | Manage the editor install |
| WebGL Build Support module | matched to 6000.3.5f2 | Required build target |
| Node.js | **18+** | Run the backend locally |
| npm | bundled with Node | Install backend dependencies |
| MongoDB | Atlas cluster or local 6.x+ | Persistence store |
| Git | any recent | Clone the repo and Unity package URLs |

Unity packages used (resolved automatically from `Packages/manifest.json`): Universal Render Pipeline 17.3, 2D toolset (Animation, Aseprite, PSD Importer, Sprite Shape, Tilemap, Tooling), Input System 1.17, TextMeshPro (via UGUI 2.0), Visual Scripting, plus three Git packages — `com.coplaydev.unity-mcp`, `com.nobi.roundedcorners`, and `com.phengine.thaitextcare`.

---

## Installation

### 1. Clone the repository

```bash
git clone <repo-url> patayafood
cd patayafood
```

### 2. Open the Unity client

1. Launch **Unity Hub** and click **Add → Add project from disk**, selecting the repository root.
2. Make sure **Unity 6000.3.5f2** with the **WebGL** module is installed.
3. Open the project. The first import will fetch the Git-based packages — allow several minutes the first time.

### 3. Install and run the backend

```bash
cd backend
npm install
```

Create a `.env` file in `backend/` with your MongoDB connection string:

```env
MONGODB_URI=mongodb+srv://<user>:<password>@<cluster>/<db>?retryWrites=true&w=majority
PORT=3000
```

Start the API:

```bash
npm start
# → Server running on http://localhost:3000
```

### 4. Point the Unity client at your backend (optional)

The client defaults to the public deployment `https://food-theta-nine.vercel.app/`. To target your local backend, either:

- Open the **MongoDBService** GameObject in any scene and set its **Api Base Url** field, or
- Call `MongoDBService.ApiBaseUrl = "http://localhost:3000";` once at startup.

---

## Usage

### Playing the game in the Editor

1. Open `Assets/Scenes/EnterNameScene.unity`.
2. Press **Play**.
3. Type a player name — the client will fetch any existing record or create a new one, then load `Scene_Selector` (the hub).
4. Select an unlocked stage from the hub. Locked stages appear greyed out until their prerequisite stage is completed.

### Building for WebGL

1. **File → Build Profiles → WebGL → Switch Platform**.
2. Add the scenes in order: `EnterNameScene`, `Scene_Selector`, `Scene01_Fish_Info` … `Scene11_Sterilize`, `LeaderBoard`.
3. **Build** to an output folder, then host the resulting `index.html` + `Build/` directory on any static web host.

### Backend API reference

All routes are prefixed `/api`.

| Method | Path | Purpose |
| --- | --- | --- |
| `POST` | `/api/scores` | Upsert a player record by `playerName`. The server recomputes `totalScore` from the four sub-scores. |
| `GET` | `/api/player/:name` | Returns `{ exists, data }` for a single player. |
| `GET` | `/api/scores?playerName=…` | Top-3 leaderboard plus the player's rank and the player immediately above them. |

Example payload for `POST /api/scores`:

```json
{
  "playerName": "Alice",
  "fishSelectionScore": 80,
  "fishPrepScore": 4,
  "fishCheckTempScore": 30,
  "fishPackagingScore": 3,
  "stageCount": 10
}
```

### Deploying the backend to Vercel

`backend/vercel.json` already routes all traffic to `server.js`. When `process.env.VERCEL` is set the entrypoint exports the Express app instead of listening on a port — no code changes needed.

```bash
cd backend
vercel deploy
```

Set `MONGODB_URI` in the Vercel project's environment variables.

---

## Project Structure

```
food/
├── Assets/
│   ├── Scenes/                   # 11 stage scenes + EnterName, Scene_Selector, LeaderBoard
│   ├── Scripts/
│   │   ├── setting/              # Shared cross-scene plumbing
│   │   │   ├── StateManager.cs        # Static in-memory session state and scoring
│   │   │   ├── MongoDBService.cs      # Singleton REST client (UnityWebRequest)
│   │   │   ├── EnterNameManager.cs    # Player login / profile rehydrate
│   │   │   ├── LeaderBoardManager.cs  # Pulls and displays the leaderboard
│   │   │   ├── Loading.cs             # Singleton loading overlay
│   │   │   ├── StageLock.cs           # Disables hub buttons until prerequisites met
│   │   │   └── StarDisplay.cs         # 0–3 star UI helper
│   │   ├── Fish_Info/            # Scene 01 — informational pop-ups
│   │   ├── Fish_Selection/       # Scene 02 — scored: belt-based fish picking
│   │   ├── FIsh_Dressed/         # Scene 03 & 08 — Click_Select hygiene dressing
│   │   ├── Fish_Thaw/            # Scene 04 — thawing mini-game
│   │   ├── Fish_prep/            # Scene 05 — scored: gutting and cleaning
│   │   ├── Fish_Steaming/        # Scene 06 — timed steaming
│   │   ├── Fish_CheckTemp/       # Scene 07 — scored: temperature probe placement
│   │   ├── Fish_Trim/            # Scene 09 — timed trimming
│   │   ├── Fish_Pakaging/        # Scene 10 — scored: recipe-matching can assembly
│   │   ├── Fish_Sterilize/       # Scene 11 — final sterilize
│   │   └── SceneSelection/       # Hub-screen UI helpers (ShowStar etc.)
│   ├── Prefabs/  Image/  Music_Source/  Animation/  Settings/  …
├── backend/
│   ├── server.js                 # Express app, Mongoose schema, all 3 routes
│   ├── vercel.json               # Vercel serverless routing
│   └── package.json              # Dependencies: express, mongoose, cors, dotenv
├── Packages/manifest.json        # Unity package list (URP, 2D, Input, unity-mcp, …)
├── ProjectSettings/              # Unity project configuration (target: WebGL)
└── CLAUDE.md                     # Architecture notes for AI assistants
```

### How scoring is wired together

`StateManager` (a plain `static` C# class) holds the player name, four sub-scores, and `stageCount` for the active session. Mini-games write into it as the player progresses, and `StateManager.SendPacket()` is the canonical call that surfaces the loading overlay and `POST`s everything to the backend. The server independently recomputes `totalScore` from the four sub-scores — both sides must stay in sync if a fifth scored stage is added.

| Sub-score | Scene | Scoring rule |
| --- | --- | --- |
| `FishSelection` | Scene 02 | ±10 per correct/wrong fish pick, ±5 per correct/wrong discard, −5 per fish that escapes the belt |
| `FishPrep` | Scene 05 | +1 per completed prep step (currently maxes at 4) |
| `FishCheckTemp` | Scene 07 | +10 per correct thermometer reading; stars derived from configurable thresholds |
| `FishPackaging` | Scene 10 | 0–3 based on meat / weight / oil recipe match |

The other seven scenes are unscored and only advance `StageCount` via `EnterScene.SetCount(int)`.

---

## Known Issues / Roadmap

- **Double-increment in `Fish_Prep_Handler.SplitFish`.** The method increments `tracking_Scroe` directly *and* calls `AddingScore()`, so the split step is silently worth 2 points instead of 1, making the stage's effective max 4 instead of an expected 3. See `Assets/Scripts/Fish_prep/Fish_Prep_Handler.cs`.
- **Hardcoded API URL in source.** `MongoDBService.apiBaseUrl` defaults to a specific Vercel deployment. Consider moving this to a `ScriptableObject` config or build-time environment so QA, staging, and production builds are easier to manage.
- **No CORS allow-list on the backend.** `app.use(cors())` accepts all origins — fine for development but should be restricted to the production WebGL host before public release.
- **No backend authentication.** Anyone who knows the API URL can write any score to any `playerName`. A signed token or per-player secret would prevent leaderboard tampering.
- **`Loading.Show()` followed immediately by `Loading.Hide()` in `StateManager.SendPacket`.** The overlay is hidden synchronously after firing the request coroutine rather than from the completion callback, so it never actually appears during the network round-trip.
- **Two `Get_Dressed` scenes (03 and 08)** share the same folder name (`FIsh_Dressed`, with a typo). Renaming to `Fish_Dressed` and splitting per-scene logic would tidy the structure.
- **`Fish_Sterilize` scripts folder is empty.** Scene 11 currently has no gameplay scripts of its own.
- **Field-name duplication across three layers.** Sub-score keys are repeated in `StateManager`, `MongoDBService.ScorePayload`/`PlayerData`, and the Mongoose `scoreSchema`. A shared schema definition (or codegen) would prevent silent drift since `JsonUtility` and Mongoose both key off the literal field name.
- **No automated tests** on either the Unity or backend side.

---

## License

Proprietary — © Patayafood Company / PickledSerpent. All rights reserved.
