#if FIREBASE_ENABLED
using UnityEngine;

/// <summary>
/// Populates the global leaderboard panel. Refreshes automatically when the panel is
/// enabled; also hook <see cref="Refresh"/> to a Refresh button. Spawns one
/// <c>rowPrefab</c> (carrying a <see cref="LeaderboardRowUI"/>) per entry under content.
/// </summary>
public class LeaderboardUI : MonoBehaviour
{
    [SerializeField] private Transform content;
    [SerializeField] private GameObject rowPrefab;
    [SerializeField] private int topN = 10;

    private void OnEnable()
    {
        Refresh();
    }

    public void Refresh()
    {
        if (LeaderboardManager.Instance == null || content == null || rowPrefab == null) return;

        LeaderboardManager.Instance.FetchTop(topN, entries =>
        {
            foreach (Transform child in content) Destroy(child.gameObject);

            int rank = 1;
            foreach (LeaderboardEntry e in entries)
            {
                GameObject go = Instantiate(rowPrefab, content);
                LeaderboardRowUI row = go.GetComponent<LeaderboardRowUI>();
                if (row != null) row.SetData(rank, e.displayName, e.score);
                rank++;
            }
        });
    }
}
#endif
