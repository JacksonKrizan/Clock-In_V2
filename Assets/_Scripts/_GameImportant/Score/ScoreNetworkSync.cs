using UnityEngine;
using Photon.Pun;
using Hashtable = ExitGames.Client.Photon.Hashtable;

/// <summary>
/// Broadcasts THIS player's score to everyone else in the Photon room. It listens to
/// ScoreManager.OnScoreChanged and writes the score into the player's Photon Custom
/// Properties, which Photon automatically syncs to all other clients.
///
/// Put this component on the same object as <see cref="ScoreManager"/> (it persists
/// across scenes too). <see cref="LiveScoreboardUI"/> reads these values to draw the board.
/// </summary>
public class ScoreNetworkSync : MonoBehaviourPunCallbacks
{
    /// <summary>The key used to store the score in Photon Custom Properties.</summary>
    public const string ScoreKey = "score";

    private void Start()
    {
        if (ScoreManager.Instance != null)
            ScoreManager.Instance.OnScoreChanged += PushScore;
    }

    private void OnDestroy()
    {
        if (ScoreManager.Instance != null)
            ScoreManager.Instance.OnScoreChanged -= PushScore;
    }

    // When we (re)join a room, publish our current score so others see it immediately.
    public override void OnJoinedRoom()
    {
        if (ScoreManager.Instance != null)
            PushScore(ScoreManager.Instance.CurrentScore);
    }

    private void PushScore(int score)
    {
        if (!PhotonNetwork.InRoom) return; // Custom properties only sync inside a room.
        PhotonNetwork.LocalPlayer.SetCustomProperties(new Hashtable { { ScoreKey, score } });
    }
}
