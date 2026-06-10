using UnityEngine;
using TMPro;

/// <summary>
/// One row in the leaderboard list. Put this on the row prefab and assign the three
/// text fields. <see cref="LeaderboardUI"/> calls <see cref="SetData"/> per entry.
/// (No backend dependency, so this compiles before the Firebase SDK is imported.)
/// </summary>
public class LeaderboardRowUI : MonoBehaviour
{
    [SerializeField] private TMP_Text rankText;
    [SerializeField] private TMP_Text nameText;
    [SerializeField] private TMP_Text scoreText;

    public void SetData(int rank, string playerName, int score)
    {
        if (rankText != null) rankText.text = rank.ToString();
        if (nameText != null) nameText.text = playerName;
        if (scoreText != null) scoreText.text = score.ToString();
    }
}
