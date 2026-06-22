using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Photon.Pun;
using Photon.Realtime;

// On-screen scoreboard. Toggle with T. Lists every player by score, live.
// Self-builds its own Canvas/text, so there is nothing to set up in the editor.
public class Scoreboard : MonoBehaviourPunCallbacks
{
    public static Scoreboard Instance;

    GameObject panel;
    TMP_Text text;
    bool visible;
    public bool isVisible;

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    static void Bootstrap()
    {
        if (Instance == null)
            new GameObject("Scoreboard").AddComponent<Scoreboard>();
    }

    void Awake()
    {
        if (Instance && Instance != this) { Destroy(gameObject); return; }
        Instance = this;
        DontDestroyOnLoad(gameObject);
        BuildUI();
        SetVisible(false);
    }

    void Start()
    {
        if (ScoreManager.Instance != null)
            ScoreManager.Instance.OnScoreChanged += HandleScoreChanged;
    }

    void OnDestroy()
    {
        if (ScoreManager.Instance != null)
            ScoreManager.Instance.OnScoreChanged -= HandleScoreChanged;
    }

    void Update()
    {
        if(visible == true)
        {
            isVisible = true;
        }
        else
        {
            isVisible = false;
        }

        // T toggles the board (only in a room, so it can't clash with menu text input)
        if (Input.GetKeyDown(KeyCode.T) && PhotonNetwork.InRoom)
            SetVisible(!visible);
    }

    void SetVisible(bool v)
    {
        visible = v;
        if (panel != null) panel.SetActive(v);
        if (v) Refresh();
    }

    void HandleScoreChanged(Player p, int s) { if (visible) Refresh(); }
    public override void OnPlayerEnteredRoom(Player newPlayer) { if (visible) Refresh(); }
    public override void OnPlayerLeftRoom(Player otherPlayer) { if (visible) Refresh(); }
    public override void OnJoinedRoom() { if (visible) Refresh(); }

    void Refresh()
    {
        if (text == null) return;

        var sb = new StringBuilder();
        sb.AppendLine("<b>SCOREBOARD</b>");
        sb.AppendLine();

        if (PhotonNetwork.InRoom)
        {
            var ranked = PhotonNetwork.PlayerList.OrderByDescending(GetScore);
            foreach (Player p in ranked)
            {
                string name = string.IsNullOrEmpty(p.NickName) ? ("Player " + p.ActorNumber) : p.NickName;
                sb.AppendLine($"{name,-18}{GetScore(p)}");
            }
        }
        else
        {
            sb.AppendLine("(not in a room)");
        }

        text.text = sb.ToString();
    }

    int GetScore(Player p)
    {
        return ScoreManager.Instance != null ? ScoreManager.Instance.GetScore(p) : 0;
    }

    void BuildUI()
    {
        var canvasGO = new GameObject("ScoreboardCanvas");
        canvasGO.transform.SetParent(transform);
        var canvas = canvasGO.AddComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        canvas.sortingOrder = 100;
        var scaler = canvasGO.AddComponent<CanvasScaler>();
        scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        scaler.referenceResolution = new Vector2(1920, 1080);

        panel = new GameObject("Panel");
        panel.transform.SetParent(canvasGO.transform, false);
        var bg = panel.AddComponent<Image>();
        bg.color = new Color(0f, 0f, 0f, 0.78f);
        var prt = panel.GetComponent<RectTransform>();
        prt.anchorMin = prt.anchorMax = prt.pivot = new Vector2(0.5f, 1f);
        prt.anchoredPosition = new Vector2(0f, -40f);
        prt.sizeDelta = new Vector2(460f, 320f);

        var textGO = new GameObject("Text");
        textGO.transform.SetParent(panel.transform, false);
        text = textGO.AddComponent<TextMeshProUGUI>();
        text.fontSize = 26f;
        text.color = Color.white;
        text.alignment = TextAlignmentOptions.TopLeft;
        text.enableWordWrapping = false;
        var trt = textGO.GetComponent<RectTransform>();
        trt.anchorMin = Vector2.zero;
        trt.anchorMax = Vector2.one;
        trt.offsetMin = new Vector2(24f, 20f);
        trt.offsetMax = new Vector2(-24f, -20f);
    }
}
