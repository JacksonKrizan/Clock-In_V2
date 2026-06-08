# Accounts, Scoring & Leaderboard — Setup

Scripts are split into two layers:

- **Score layer** (`_Scripts/_GameImportant/Score/`) — `ScoreManager`, `ScoreHUD`.
  No backend. **Works right now.**
- **Account/leaderboard layer** (`_Scripts/_GameImportant/Account/`) — Firebase.
  All files are wrapped in `#if FIREBASE_ENABLED`, so they are inert until you finish
  the steps below. The project compiles fine without them.

---

## 1. Score system (no setup needed)
- Put a `ScoreManager` component on a GameObject in the **menu scene** (it survives scene
  loads via `DontDestroyOnLoad`, so one instance covers the whole session).
- Call points from anywhere, any time:
  ```csharp
  ScoreManager.Instance.AddScore(10);      // award
  ScoreManager.Instance.ResetScore();      // new match
  ScoreManager.Instance.SubmitToLeaderboard();  // push to Firebase (no-op for guests)
  ```
- `PacketGoal` already calls `AddScore(pointsPerDelivery)` on delivery — set the points
  value in its Inspector.
- For an on-screen total: add `ScoreHUD` to a HUD object and assign a `TMP_Text`.

## 2. Firebase Console
1. Create a project at <https://console.firebase.google.com>.
2. **Authentication → Sign-in method → enable Email/Password.**
3. **Realtime Database → Create database** (start in test mode).
4. Add a Unity app, download **`google-services.json`**, put it in `Assets/`.
5. Database rules (replace test rules once it works):
   ```json
   {
     "rules": {
       "leaderboard": {
         ".read": true,
         "$uid": { ".write": "auth != null && auth.uid == $uid" }
       }
     }
   }
   ```

## 3. Import the Firebase Unity SDK
1. Download the Firebase Unity SDK, import **`FirebaseAuth.unitypackage`** and
   **`FirebaseDatabase.unitypackage`** (these also bring in External Dependency Manager).
2. Let it resolve dependencies (Assets → External Dependency Manager → Android Resolver
   if building for Android).

## 4. Turn the Firebase layer on
- **Project Settings → Player → Other Settings → Scripting Define Symbols**, add:
  ```
  FIREBASE_ENABLED
  ```
- Unity recompiles; the `Account/` scripts now become active.

## 5. Scene wiring (menu scene)
- Empty GameObject **`FirebaseBootstrap`** → add `FirebaseBootstrap`.
- GameObject **`AuthManager`** → add `AuthManager`.
- GameObject **`LeaderboardManager`** → add `LeaderboardManager`.
  (All three are `DontDestroyOnLoad` singletons — one each.)

**Auth panel** (gate before the room/lobby UI): add `AuthMenuUI` and assign
- `emailInput`, `passwordInput`, `displayNameInput` (TMP_InputFields)
- `statusText` (TMP_Text for errors)
- Buttons → OnClick → `AuthMenuUI.OnSignUpClicked` / `OnSignInClicked` / `OnGuestClicked`.

**Leaderboard panel**: add `LeaderboardUI` and assign
- `content` (the scroll-view Content transform)
- `rowPrefab` (a prefab with a `LeaderboardRowUI`, whose `rankText/nameText/scoreText`
  are assigned)
- a Refresh button → OnClick → `LeaderboardUI.Refresh` (optional; it auto-refreshes on open).

## How it flows
Menu → Firebase initializes → player Signs Up / Signs In (saved) **or** Continues as
Guest (local name, not saved) → display name becomes the Photon NickName → existing
room/lobby flow unchanged → gameplay calls `AddScore` → end of match call
`SubmitToLeaderboard()` (writes only for non-guests, keeping each player's best) →
Leaderboard panel shows the global top N.
