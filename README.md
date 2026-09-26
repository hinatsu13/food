# PatayaFood — Canned Fish Production Edutainment Game

An educational Unity WebGL game produced for **Patayafood Company** that walks players through the eleven stages of canned-fish manufacturing. Each stage is a self-contained mini-game — from selecting fresh tuna off a conveyor belt to sealing and sterilizing the finished can — letting players learn the real-world quality and food-safety steps behind every can on the shelf.

Four of the stages are scored. Per-player progress and high scores are persisted to a **MongoDB** backend keyed by employee ID, so players sign in with their employee ID and resume where they left off.

---

## Key Features

- **11 themed mini-game scenes** mirroring the real Patayafood production line: information briefing, fish selection, hygiene dressing, thawing, prep/gutting, steaming, temperature checks, cleanroom dressing, trimming, packaging, and sterilization.
- **Four scored sub-games** (Selection, Prep, Check-Temp, Packaging) summed into a server-computed `totalScore`.
- **Stage-gated progression** — locked stages in the hub scene unlock as players advance, driven by `StateManager.StageCount`.
- **Employee-ID player profiles** via a Node/Express + MongoDB REST API. Login is by employee ID (`personCode`) alone — any well-formed ID works (there is no roster; the organisation enforces correct IDs), and the ID is the only thing shown for a player.
- **JWT-secured score writes** — login issues a 12h token, all score updates require it, server clamps and enforces monotonic-only updates.
- **Global leaderboard** showing the top 3 plus the player's own rank and the player one rank above.
- **Reusable shared plumbing** for loading overlays, star ratings, scene unlocking, and network calls.
- **Local MongoDB via Docker** — `docker compose up -d` gives you an empty development DB; the server creates its indexes on first request.
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
| MongoDB | 7 (or Docker) | Persistence store. Local dev via the included `docker-compose.yml` |
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

### 3. Start a local MongoDB (optional, for local dev)

The included `backend/docker-compose.yml` launches MongoDB 7:

```bash
cd backend
docker compose up -d
```

The DB listens on `localhost:27017` with no authentication (dev-only — never expose it anywhere else). There is nothing to seed: the server creates the `food` database and its indexes on first request, and each player's record on their first score save. To wipe the data, use `docker compose down -v`.

If you're pointing at an existing MongoDB instead, skip Docker and set `MONGODB_URI` below — no setup is needed there either, unless its `food` database was written by the earlier roster build (`gameData` keyed by `personId`): drop that `gameData` collection first (or `docker compose down -v` for the dev volume), or its leftover docs and `personId_1` index make index creation and first saves fail with E11000 → 500.

### 4. Install and run the backend

```bash
cd backend
npm install
```

Create a `backend/.env` file:

```env
MONGODB_URI=mongodb://localhost:27017
MONGODB_DATABASE=food
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

The client defaults to `http://localhost:3000`. To target a different backend, call `MongoDBService.ApiBaseUrl = "https://...";` once at startup. (Setting the field in the Inspector does nothing: `MongoDBService` creates its own GameObject on first use, so a scene-placed copy is ignored.)

(`MongoDBService` is named for the backend; the client itself only speaks HTTP to the API.)

---

## Usage

### Playing the game in the Editor

1. Open `Assets/Scenes/EnterNameScene.unity`.
2. Press **Play**.
3. Type your **Employee ID (PersonCode)**. Any well-formed ID is accepted — letters, digits and hyphens, up to 50 characters, case-insensitive (it's stored uppercase, so `ab12` and `AB12` are the same player). Malformed IDs are rejected with "Invalid Employee ID". Existing players have their saved scores rehydrated; a new ID starts with a zeroed profile, and its record is created on its first score save.
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
| `GET` | `/api/person/:code` | none | Login. Normalizes the ID (trim, uppercase, must match `^[A-Z0-9-]{1,50}$`) and returns `{ exists, person: { personCode }, gameData, token }` with a 12h JWT; a new ID gets `exists: false` and zeros. Never writes. 400 means "malformed ID". |
| `POST` | `/api/scores` | **Bearer JWT** | Upsert game data (the first save creates the player's record). `personCode` is derived from the verified token; any `personCode` in the body is ignored. Each sub-score is clamped to `MAX_SCORES` (constants at top of `server.js`), then a single atomic update keeps the higher of the stored and new value (`$max`, monotonic) and recomputes `totalScore`. |
| `GET` | `/api/scores?personCode=…` | none | Top-3 leaderboard plus the caller's rank row and the player one rank above. Entries use camelCase keys (`personCode`, `totalScore`, …) — no names. |

Example login flow:

```bash
# 1. Login
curl http://localhost:3000/api/person/12345
# → { "success": true, "exists": false, "person": { "personCode": "12345" }, "gameData": {...}, "token": "eyJ..." }

# 2. Save scores
curl -X POST http://localhost:3000/api/scores \
  -H "Content-Type: application/json" \
  -H "Authorization: Bearer eyJ..." \
  -d '{"fishSelectionScore":80,"fishPrepScore":4,"fishCheckTempScore":30,"fishPackagingScore":3,"stageCount":10}'
```

### Testing the API in Postman

A ready-to-import collection lives at [`backend/postman/PatayaFood.postman_collection.json`](backend/postman/PatayaFood.postman_collection.json). It covers the full verification matrix: login (known ID, brand-new ID → `exists: false` with zeroed game data, malformed ID → 400), the JWT auth gate (missing header, bogus token), the impersonation defense (body `personCode` is ignored), the clamp, the monotonic-only rule, and the leaderboard (entries carry `personCode` + `totalScore`, no name fields).

To run it:

1. **Postman → File → Import** → drop the JSON file in.
2. Optional: click the imported collection → **Variables** tab and change the **Current Value** column (not just Initial Value, which Postman silently ignores at runtime) for:
   - `validPersonCode` — the player the tests write to (default `E2E-PLAYER-A`). The run leaves it at the clamp maximum (total 907), so it will top the leaderboard of whatever DB you point it at — don't use a real employee's ID.
   - `victimPersonCode` — the impersonation target (default `E2E-PLAYER-B`). It is only logged into, never written.

   Any two well-formed IDs work; there is no roster to match.
3. **Save** (Ctrl+S).
4. **Run Collection** (▶ at the top of the collection sidebar). All 11 tests should pass.

Note: tests 9 (clamp) and 10 (monotonic) are intentionally order-dependent — #9 pushes the row to the cap, then #10 confirms a lower POST can't deflate it. Run the collection top-to-bottom, not individual requests.

### Deploying the backend

The codebase still contains `vercel.json` and a Vercel export branch in `server.js`. The project moved off Vercel while it ran on an on-prem SQL Server, which Vercel can't reach. With MongoDB the branch could be revived by pointing `MONGODB_URI` at a hosted instance such as MongoDB Atlas, but that path hasn't been tested since the port. Until then, treat the canonical deployment path as "your own intranet host with Node 18+ and network access to the MongoDB instance".

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
│   │   │   ├── MongoDBService.cs      # Singleton REST client (UnityWebRequest + JWT)
│   │   │   ├── EnterNameManager.cs    # Login by employee ID (normalized; no roster)
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
│   ├── server.js                 # Express app, MongoDB client + indexes, JWT middleware, all 3 routes
│   ├── docker-compose.yml        # Local MongoDB 7 (no seed data)
│   ├── vercel.json               # Legacy — see "Deploying the backend"
│   └── package.json              # Dependencies: express, mongodb, jsonwebtoken, cors, dotenv
├── Packages/manifest.json        # Unity package list (URP, 2D, Input, unity-mcp, …)
├── ProjectSettings/              # Unity project configuration (target: WebGL)
└── CLAUDE.md                     # Architecture notes for AI assistants
```

### How scoring is wired together

`StateManager` (a plain `static` C# class) holds the player's `PersonCode` (the employee ID, which is also what's displayed), four sub-scores, and `StageCount` for the active session. Mini-games write into it as the player progresses, and `StateManager.SendPacket()` is the canonical call that surfaces the loading overlay and `POST`s everything to `/api/scores` with the active JWT. The server clamps each sub-score, applies monotonic-only update rules, and stores `totalScore` on the player's `gameData` document (created by their first save), recomputed in the same atomic update as the sub-scores.

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

1. **JWT-bound identity on writes.** The body of `POST /api/scores` never identifies the player — the `personCode` is pulled from a signed token issued at login. A tampered `personCode` in the body is silently ignored.
2. **Server-side clamp + monotonic-only updates.** Each sub-score is clamped to a configured maximum (`MAX_SCORES` at the top of `server.js`) and the database update (`$max`) only changes a field when the incoming value is *greater than* what's stored. Once a real high score is set, it cannot be lowered or replaced by garbage.

**What this does NOT defend against.** There is no password, so the token only binds a session to whatever ID was typed at login — anyone can play as any ID (a colleague's, or one that doesn't exist), which the organisation accepted since it enforces correct IDs out-of-band. JWT only prevents tampering *after* login, not the login itself. Combined with the clamp + monotonic rule the worst-case impact is "someone sets a victim's score to the legitimate maximum", which is acceptable for an intranet edutainment leaderboard. If real authentication is needed later, only the `GET /api/person/:code` handler needs to grow a password/SSO check.

---

## Known Issues / Roadmap

- **WebGL token loss on refresh.** The JWT lives only in a C# static field inside the WebAssembly heap, so refreshing the page forces a fresh login. Persisting it to `localStorage` via a `.jslib` plugin (or moving to an `HttpOnly` cookie) would smooth this over but trades off some security; default position is to leave it as-is for the single-sitting play session use case.
- **Weak login.** The employee ID is the only credential and isn't checked against any roster, so logging in as a colleague only requires knowing their ID, and a mistyped ID becomes a separate player once it saves a score (login alone writes nothing). The server-side clamp bounds the damage; adding a second factor (SSO / Entra ID / password) would close the gap.
- **`MAX_SCORES` must track gameplay.** The caps (500 / 4 / 400 / 3, `stageCount` 11) match the current observed maximums; if a scene's scoring changes, raise its cap or legitimate scores get silently clamped.
- **`Loading.Show()` followed immediately by `Loading.Hide()` in `StateManager.SendPacket`.** The overlay is hidden synchronously after firing the request coroutine rather than from the completion callback, so it never actually appears during the network round-trip.
- **Double-increment in `Fish_Prep_Handler.SplitFish`.** The method increments `tracking_Scroe` directly *and* calls `AddingScore()`, so the split step is silently worth 2 points instead of 1.
- **Two `Get_Dressed` scenes (03 and 08)** share the same folder name (`FIsh_Dressed`, with a typo). Renaming to `Fish_Dressed` and splitting per-scene logic would tidy the structure.
- **Field-name duplication across three layers.** Sub-score keys are repeated in `StateManager`, `MongoDBService.ScorePayload`/`GameData`, and the field lists in `server.js`. `JsonUtility` and the server's update both key off literal names, so a typo silently drops data.
- **No CORS allow-list.** `app.use(cors())` accepts all origins. Lock down to the deployed WebGL host before public release.
- **No automated tests** on either the Unity or backend side.

---

## License

Proprietary — © Patayafood Company / PickledSerpent. All rights reserved.
