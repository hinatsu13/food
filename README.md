# PatayaFood — Canned Fish Production Edutainment Game

An educational Unity WebGL game produced for **Patayafood Company** that walks players through the eleven stages of canned-fish manufacturing. Each stage is a self-contained mini-game — from selecting fresh tuna off a conveyor belt to sealing and sterilizing the finished can — letting players learn the real-world quality and food-safety steps behind every can on the shelf.

Four of the stages are scored. Per-player progress and high scores are persisted to a **SQL Server** backend that joins against the company's existing HRIS roster, so players sign in with their employee ID and resume where they left off.

---

## Key Features

- **11 themed mini-game scenes** mirroring the real Patayafood production line: information briefing, fish selection, hygiene dressing, thawing, prep/gutting, steaming, temperature checks, cleanroom dressing, trimming, packaging, and sterilization.
- **Four scored sub-games** (Selection, Prep, Check-Temp, Packaging) summed into a server-computed `TotalScore`.
- **Stage-gated progression** — locked stages in the hub scene unlock as players advance, driven by `StateManager.StageCount`.
- **HRIS-backed player profiles** via a Node/Express + SQL Server REST API. Login is by `PersonCode` (employee ID); only roster members can play.
- **JWT-secured score writes** — login issues a 12h token, all score updates require it, server clamps and enforces monotonic-only updates.
- **Global leaderboard** showing the top 3 plus the player's own rank and the player one rank above.
- **Reusable shared plumbing** for loading overlays, star ratings, scene unlocking, and network calls.
- **Local SQL Server via Docker** — `docker compose up -d` gives you a development DB with init scripts.
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
| SQL Server | 2019+ (or Docker) | Persistence store. Local dev via the included `docker-compose.yml` |
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

### 3. Start a local SQL Server (optional, for local dev)

The included `backend/docker-compose.yml` launches SQL Server 2019 Express with init scripts in `backend/docker/mssql/init/`:

```bash
cd backend
docker compose up -d
```

The DB listens on `localhost:1433`, SA password `Local_Dev_2012!` (dev-only — never use this anywhere else). To wipe the data volume and re-run init, use `docker compose down -v`.

If you're pointing at an existing HRIS database instead, skip Docker and just configure the env vars below.

### 4. Install and run the backend

```bash
cd backend
npm install
```

Create a `backend/.env` file:

```env
MSSQL_USER=sa
MSSQL_PASSWORD=Local_Dev_2012!
MSSQL_HOST=localhost
MSSQL_PORT=1433
MSSQL_DATABASE=HRIS
MSSQL_ENCRYPT=false
JWT_SECRET=<paste long random string here>
PORT=3000
```

Generate a `JWT_SECRET` with:

```bash
node -e "console.log(require('crypto').randomBytes(48).toString('base64'))"
```

The server **refuses to start without `JWT_SECRET`** — keep it stable across restarts (rotating it logs everyone out instantly) and never commit it.

Start the API:

```bash
npm start
# → Server running on http://localhost:3000
```

### 5. Point the Unity client at your backend (optional)

The client defaults to `http://localhost:3000`. To target a different backend, either:

- Open the **MSSqlService** GameObject in any scene and set its **Api Base Url** field, or
- Call `MSSqlService.ApiBaseUrl = "https://...";` once at startup.

---

## Usage

### Playing the game in the Editor

1. Open `Assets/Scenes/EnterNameScene.unity`.
2. Press **Play**.
3. Type your **Employee ID (PersonCode)**. The client validates against the HRIS roster — unknown IDs are rejected with "Employee ID not found". Existing players have their saved scores rehydrated; first-time roster members start with a zeroed profile.
4. Select an unlocked stage from `Scene_Selector`. Locked stages appear greyed out until their prerequisite stage is completed.

### Building for WebGL

1. **File → Build Profiles → WebGL → Switch Platform**.
2. Add the scenes in order: `EnterNameScene`, `Scene_Selector`, `Scene01_Fish_Info` … `Scene11_Sterilize`, `LeaderBoard`.
3. **Build** to an output folder, then host the resulting `index.html` + `Build/` directory on any static web host.
4. **CORS**: once deployed to a real host, replace the wide-open `app.use(cors())` in `server.js` with an allowlist of the game's origin.
5. **Mixed content**: if the game is served over HTTPS, the API must be HTTPS too — browsers silently block HTTP-from-HTTPS requests.

### Backend API reference

All routes are prefixed `/api`.

| Method | Path | Auth | Purpose |
| --- | --- | --- | --- |
| `GET` | `/api/person/:code` | none | Login. Looks up `PersonCode` in the HRIS roster; on success returns `{ person, gameData, token }` with a 12h JWT. 404 means "not in roster". |
| `POST` | `/api/scores` | **Bearer JWT** | Upsert game data. `PersonID` is derived from the verified token; any `personCode` in the body is ignored. Each sub-score is clamped to `MAX_SCORES` (constants at top of `server.js`) and the SQL `MERGE` only overwrites a column when the new value is greater (monotonic). |
| `GET` | `/api/scores?personCode=…` | none | Top-3 leaderboard plus the caller's rank row and the player one rank above. |

Example login flow:

```bash
# 1. Login
curl http://localhost:3000/api/person/12345
# → { "success": true, "exists": true, "person": {...}, "gameData": {...}, "token": "eyJ..." }

# 2. Save scores
curl -X POST http://localhost:3000/api/scores \
  -H "Content-Type: application/json" \
  -H "Authorization: Bearer eyJ..." \
  -d '{"fishSelectionScore":80,"fishPrepScore":4,"fishCheckTempScore":30,"fishPackagingScore":3,"stageCount":10}'
```

### Deploying the backend

The codebase still contains `vercel.json` and a Vercel export branch in `server.js`, but the project has moved off Vercel — Vercel can't reach an on-prem SQL Server, and the production deployment now runs on internal infrastructure. The Vercel files are kept only for historical reference; treat the canonical deployment path as "your own intranet host with Node 18+ and network access to the HRIS database".

Whatever you deploy on, set every env var from step 4 of installation. Especially `JWT_SECRET` — without it the server won't boot.

---

## Project Structure

```
food/
├── Assets/
│   ├── Scenes/                   # 11 stage scenes + EnterName, Scene_Selector, LeaderBoard
│   ├── Scripts/
│   │   ├── setting/              # Shared cross-scene plumbing
│   │   │   ├── StateManager.cs        # Static in-memory session state and scoring
│   │   │   ├── MSSqlService.cs        # Singleton REST client (UnityWebRequest + JWT)
│   │   │   ├── EnterNameManager.cs    # Login by PersonCode against the HRIS roster
│   │   │   ├── LeaderBoardManager.cs  # Pulls and displays the leaderboard
│   │   │   ├── Loading.cs             # Singleton loading overlay
│   │   │   ├── StageLock.cs           # Disables hub buttons until prerequisites met
│   │   │   └── StarDisplay.cs         # 0–3 star UI helper
│   │   ├── Fish_Info/            # Scene 01 — informational pop-ups
│   │   ├── Fish_Selection/       # Scene 02 — scored: belt-based fish picking
│   │   ├── FIsh_Dressed/         # Scene 03 & 08 — hygiene dressing
│   │   ├── Fish_Thaw/            # Scene 04 — thawing mini-game
│   │   ├── Fish_prep/            # Scene 05 — scored: gutting and cleaning
│   │   ├── Fish_Steaming/        # Scene 06 — timed steaming
│   │   ├── Fish_CheckTemp/       # Scene 07 — scored: temperature probe placement
│   │   ├── Fish_Trim/            # Scene 09 — timed trimming
│   │   ├── Fish_Pakaging/        # Scene 10 — scored: recipe-matching can assembly
│   │   └── SceneSelection/       # Hub-screen UI helpers (ShowStar etc.)
│   ├── Prefabs/  Image/  Music_Source/  Animation/  Settings/  …
├── backend/
│   ├── server.js                 # Express app, mssql pool, JWT middleware, all 3 routes
│   ├── docker-compose.yml        # Local SQL Server 2019 Express
│   ├── docker/mssql/init/        # First-boot SQL init scripts
│   ├── vercel.json               # Legacy — see "Deploying the backend"
│   └── package.json              # Dependencies: express, mssql, jsonwebtoken, cors, dotenv
├── Packages/manifest.json        # Unity package list (URP, 2D, Input, unity-mcp, …)
├── ProjectSettings/              # Unity project configuration (target: WebGL)
└── CLAUDE.md                     # Architecture notes for AI assistants
```

### How scoring is wired together

`StateManager` (a plain `static` C# class) holds the player's `PersonCode`, display name, four sub-scores, and `StageCount` for the active session. Mini-games write into it as the player progresses, and `StateManager.SendPacket()` is the canonical call that surfaces the loading overlay and `POST`s everything to `/api/scores` with the active JWT. The server clamps each sub-score, applies monotonic-only update rules, and `TotalScore` is a computed column on the `PersonGameData` table.

| Sub-score | Scene | Scoring rule |
| --- | --- | --- |
| `FishSelection` | Scene 02 | ±10 per correct/wrong fish pick, ±5 per correct/wrong discard, −5 per fish that escapes the belt |
| `FishPrep` | Scene 05 | +1 per completed prep step (currently maxes at 4) |
| `FishCheckTemp` | Scene 07 | +10 per correct thermometer reading; stars derived from configurable thresholds |
| `FishPackaging` | Scene 10 | 0–3 based on meat / weight / oil recipe match |

The other seven scenes are unscored and only advance `StageCount` via `EnterScene.SetCount(int)`.

---

## Security model

The intranet-only threat model is "casual cheating by employees who know how to use curl", not external attackers. Two server-side defenses are enforced:

1. **JWT-bound identity on writes.** The body of `POST /api/scores` never identifies the player — `PersonID` is pulled from a signed token issued at login. A tampered `personCode` in the body is silently ignored.
2. **Server-side clamp + monotonic-only updates.** Each sub-score is clamped to a configured maximum (`MAX_SCORES` at the top of `server.js`) and the SQL `MERGE` only overwrites a column when the incoming value is *greater than* what's stored. Once a real high score is set, it cannot be lowered or replaced by garbage.

**What this does NOT defend against.** PersonCode-only login means anyone who knows a colleague's employee ID can call `/api/person/:code` and obtain a token for them — JWT only prevents tampering *after* login, not the login itself. Combined with the clamp + monotonic rule the worst-case impact is "someone sets a victim's score to the legitimate maximum", which is acceptable for an intranet edutainment leaderboard. If real authentication is needed later, only the `GET /api/person/:code` handler needs to grow a password/SSO check.

---

## Known Issues / Roadmap

- **WebGL token loss on refresh.** The JWT lives only in a C# static field inside the WebAssembly heap, so refreshing the page forces a fresh login. Persisting it to `localStorage` via a `.jslib` plugin (or moving to an `HttpOnly` cookie) would smooth this over but trades off some security; default position is to leave it as-is for the single-sitting play session use case.
- **Weak login.** PersonCode is the only credential, so logging in as a colleague only requires knowing their employee ID. The server-side clamp bounds the damage; adding a second factor (SSO / Entra ID / password) would close the gap.
- **`MAX_SCORES` are placeholder caps.** Each sub-score is capped at 1000 — loose enough that no legitimate score should hit it, but tune downward once real gameplay maximums are observed.
- **`Loading.Show()` followed immediately by `Loading.Hide()` in `StateManager.SendPacket`.** The overlay is hidden synchronously after firing the request coroutine rather than from the completion callback, so it never actually appears during the network round-trip.
- **Double-increment in `Fish_Prep_Handler.SplitFish`.** The method increments `tracking_Scroe` directly *and* calls `AddingScore()`, so the split step is silently worth 2 points instead of 1.
- **Two `Get_Dressed` scenes (03 and 08)** share the same folder name (`FIsh_Dressed`, with a typo). Renaming to `Fish_Dressed` and splitting per-scene logic would tidy the structure.
- **Field-name duplication across three layers.** Sub-score keys are repeated in `StateManager`, `MSSqlService.ScorePayload`/`GameData`, and the SQL columns. `JsonUtility` and the SQL `MERGE` both key off literal names, so a typo silently drops data.
- **No CORS allow-list.** `app.use(cors())` accepts all origins. Lock down to the deployed WebGL host before public release.
- **No automated tests** on either the Unity or backend side.

---

## License

Proprietary — © Patayafood Company / PickledSerpent. All rights reserved.
