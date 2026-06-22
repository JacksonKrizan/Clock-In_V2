using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using Hashtable = ExitGames.Client.Photon.Hashtable;

// Networked score. Call from anywhere: ScoreManager.Instance.AddScore(10);
// Scores live on each player's Photon CustomProperties, so they sync to every
// client automatically and survive scene loads. No PhotonView on this object.
public class ScoreManager : MonoBehaviourPunCallbacks
{
    public static ScoreManager Instance;

    const string SCORE_KEY = "score";

    // fires on EVERY client whenever any player's score changes
    public System.Action<Player, int> OnScoreChanged;

    // Auto-create the manager before the first scene loads — no setup in the editor.
    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    static void Bootstrap()
    {
        if (Instance == null)
        {
            new GameObject("ScoreManager").AddComponent<ScoreManager>();
        }
    }

    void Awake()
    {
        if (Instance && Instance != this) { Destroy(gameObject); return; }
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    // add points to the local player
    public void AddScore(int points)
    {
        AddScore(PhotonNetwork.LocalPlayer, points);
    }

    // add points to a specific player
    public void AddScore(Player player, int points)
    {
        if (player == null) return;
        player.SetCustomProperties(new Hashtable { { SCORE_KEY, GetScore(player) + points } });
    }

    // read the local player's score
    public int GetScore()
    {
        return GetScore(PhotonNetwork.LocalPlayer);
    }

    // read any player's score
    public int GetScore(Player player)
    {
        if (player != null && player.CustomProperties.TryGetValue(SCORE_KEY, out object v))
            return (int)v;
        return 0;
    }

    // zero out the local player's score (e.g. start of a new game)
    public void ResetScore()
    {
        PhotonNetwork.LocalPlayer.SetCustomProperties(new Hashtable { { SCORE_KEY, 0 } });
    }

    // start each room at 0
    public override void OnJoinedRoom()
    {
        ResetScore();
    }

    // relay any player's score change to listeners (UI, scoreboard, etc.)
    public override void OnPlayerPropertiesUpdate(Player target, Hashtable changedProps)
    {
        if (changedProps.ContainsKey(SCORE_KEY))
            OnScoreChanged?.Invoke(target, GetScore(target));
    }
}
