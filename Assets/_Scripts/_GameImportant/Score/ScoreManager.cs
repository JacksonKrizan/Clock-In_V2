using System;
using UnityEngine;

/// <summary>
/// Central score tracker. Call <c>ScoreManager.Instance.AddScore(points)</c> from
/// anywhere, at any time, to award points. Survives scene loads so the menu and the
/// gameplay scenes share one running total.
///
/// This script has NO backend dependency. To record a score on the global leaderboard
/// the Firebase LeaderboardManager subscribes to <see cref="OnSubmitRequested"/>, so
/// gameplay code only ever has to talk to ScoreManager.
/// </summary>
public class ScoreManager : MonoBehaviour
{
    public static ScoreManager Instance { get; private set; }

    [SerializeField] private int startingScore = 0;

    [Tooltip("When on, every score change tries to save your best to the leaderboard " +
             "automatically (no-op for guests). Turn off if you'd rather submit only at " +
             "the end of a match via SubmitToLeaderboard().")]
    [SerializeField] private bool autoSubmitOnChange = true;

    /// <summary>The player's current running score.</summary>
    public int CurrentScore { get; private set; }

    /// <summary>Fires whenever the score changes. A HUD text can subscribe to this.</summary>
    public event Action<int> OnScoreChanged;

    /// <summary>
    /// Fires when <see cref="SubmitToLeaderboard"/> is called. The Firebase
    /// LeaderboardManager subscribes so ScoreManager itself stays backend-free.
    /// </summary>
    public event Action<int> OnSubmitRequested;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);
        CurrentScore = startingScore;
    }

    /// <summary>Add points (use a negative number to subtract). Safe to call any time.</summary>
    public void AddScore(int points)
    {
        CurrentScore += points;
        OnScoreChanged?.Invoke(CurrentScore);
        Debug.Log($"[ScoreManager] {(points >= 0 ? "+" : "")}{points} -> {CurrentScore}");

        // Save progress automatically so scores actually reach the leaderboard. The
        // leaderboard layer ignores this for guests and only writes a new personal best.
        if (autoSubmitOnChange) SubmitToLeaderboard();
    }

    /// <summary>Reset the running total back to the starting score (e.g. a new match).</summary>
    public void ResetScore()
    {
        CurrentScore = startingScore;
        OnScoreChanged?.Invoke(CurrentScore);
    }

    /// <summary>
    /// Ask the leaderboard layer to record the current score. Does nothing beyond firing
    /// the event if no leaderboard is listening (or the player is a guest).
    /// </summary>
    public void SubmitToLeaderboard()
    {
        OnSubmitRequested?.Invoke(CurrentScore);
    }
}
