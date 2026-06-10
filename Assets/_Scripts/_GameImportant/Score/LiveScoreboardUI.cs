using System.Linq;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using Hashtable = ExitGames.Client.Photon.Hashtable;

/// <summary>
/// Draws a live, in-match scoreboard of EVERY player in the room, sorted highest-first.
/// Reads each player's score from their Photon Custom Properties (published by
/// <see cref="ScoreNetworkSync"/>) and rebuilds whenever a score changes or a player
/// joins/leaves. Reuses the <see cref="LeaderboardRowUI"/> row prefab.
///
/// Put this on a scoreboard panel in the gameplay scene; assign the scroll-view content
/// transform and a row prefab.
/// </summary>
public class LiveScoreboardUI : MonoBehaviourPunCallbacks
{
    [SerializeField] private Transform content;
    [SerializeField] private GameObject rowPrefab; // must carry a LeaderboardRowUI

    private void Start()
    {
        Refresh();
    }

    // Photon callbacks: refresh the board whenever anything relevant changes.
    public override void OnPlayerPropertiesUpdate(Player targetPlayer, Hashtable changedProps) => Refresh();
    public override void OnPlayerEnteredRoom(Player newPlayer) => Refresh();
    public override void OnPlayerLeftRoom(Player otherPlayer) => Refresh();

    public void Refresh()
    {
        if (content == null || rowPrefab == null || !PhotonNetwork.InRoom) return;

        // Clear old rows.
        foreach (Transform child in content) Destroy(child.gameObject);

        // Sort all players in the room by score, highest first.
        Player[] players = PhotonNetwork.PlayerList.OrderByDescending(GetScore).ToArray();

        int rank = 1;
        foreach (Player p in players)
        {
            GameObject go = Instantiate(rowPrefab, content);
            LeaderboardRowUI row = go.GetComponent<LeaderboardRowUI>();
            if (row != null) row.SetData(rank, p.NickName, GetScore(p));
            rank++;
        }
    }

    private int GetScore(Player p)
    {
        if (p.CustomProperties.TryGetValue(ScoreNetworkSync.ScoreKey, out object val) && val != null)
            return (int)val;
        return 0;
    }
}
