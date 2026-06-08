#if FIREBASE_ENABLED
using System;
using System.Collections.Generic;
using UnityEngine;
using Firebase.Database;
using Firebase.Extensions;

/// <summary>A single global leaderboard entry.</summary>
[Serializable]
public struct LeaderboardEntry
{
    public string uid;
    public string displayName;
    public int score;
}

/// <summary>
/// Writes scores to and reads the global leaderboard from Firebase Realtime Database.
/// Guests are never written. Subscribes to <see cref="ScoreManager.OnSubmitRequested"/>
/// so gameplay only needs to call <c>ScoreManager.Instance.SubmitToLeaderboard()</c>.
/// </summary>
public class LeaderboardManager : MonoBehaviour
{
    public static LeaderboardManager Instance { get; private set; }

    private DatabaseReference db;

    private void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(gameObject); return; }
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    private void Start()
    {
        if (FirebaseBootstrap.IsReady) Init();
        else if (FirebaseBootstrap.Instance != null) FirebaseBootstrap.Instance.OnReady += Init;

        if (ScoreManager.Instance != null)
            ScoreManager.Instance.OnSubmitRequested += SubmitScore;
    }

    private void OnDestroy()
    {
        if (ScoreManager.Instance != null)
            ScoreManager.Instance.OnSubmitRequested -= SubmitScore;
    }

    private void Init()
    {
        db = FirebaseDatabase.DefaultInstance.RootReference;
    }

    /// <summary>
    /// Record a score for the signed-in player, keeping their best. No-op for guests
    /// or when not signed in.
    /// </summary>
    public void SubmitScore(int score)
    {
        AuthManager auth = AuthManager.Instance;
        if (auth == null || !auth.IsSignedIn || auth.IsGuest)
        {
            Debug.Log("[LeaderboardManager] Guest or not signed in - score not submitted.");
            return;
        }
        if (db == null) { Debug.LogWarning("[LeaderboardManager] DB not ready."); return; }

        DatabaseReference entryRef = db.Child("leaderboard").Child(auth.CurrentUid);

        // Only overwrite if this beats the player's stored best.
        entryRef.Child("score").GetValueAsync().ContinueWithOnMainThread(task =>
        {
            int best = 0;
            if (task.IsCompleted && task.Result != null && task.Result.Value != null)
                int.TryParse(task.Result.Value.ToString(), out best);

            if (score <= best)
            {
                Debug.Log($"[LeaderboardManager] {score} did not beat best {best}.");
                return;
            }

            var data = new Dictionary<string, object>
            {
                { "displayName", auth.DisplayName },
                { "score", score },
                { "updatedAt", ServerValue.Timestamp },
            };
            entryRef.UpdateChildrenAsync(data).ContinueWithOnMainThread(_ =>
                Debug.Log($"[LeaderboardManager] Submitted score {score}."));
        });
    }

    /// <summary>Fetch the global top N entries, highest score first.</summary>
    public void FetchTop(int n, Action<List<LeaderboardEntry>> onResult)
    {
        if (db == null) { onResult?.Invoke(new List<LeaderboardEntry>()); return; }

        db.Child("leaderboard").OrderByChild("score").LimitToLast(n)
          .GetValueAsync().ContinueWithOnMainThread(task =>
        {
            var list = new List<LeaderboardEntry>();
            if (task.IsCompleted && task.Result != null)
            {
                foreach (DataSnapshot child in task.Result.Children)
                {
                    var entry = new LeaderboardEntry { uid = child.Key };
                    object nameVal = child.Child("displayName").Value;
                    object scoreVal = child.Child("score").Value;
                    entry.displayName = nameVal != null ? nameVal.ToString() : "Unknown";
                    int s = 0; if (scoreVal != null) int.TryParse(scoreVal.ToString(), out s);
                    entry.score = s;
                    list.Add(entry);
                }
            }
            // OrderByChild + LimitToLast returns ascending; reverse for highest-first.
            list.Reverse();
            onResult?.Invoke(list);
        });
    }
}
#endif
