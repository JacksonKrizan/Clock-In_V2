using System.Collections;
using UnityEngine;
using TMPro;
using Photon.Pun;
using Photon.Realtime;

// Shared HUD for every mini-game. Put this on an ALWAYS-ACTIVE object (e.g. the
// Canvas) and assign the panel it should show/hide plus the 3 TMP boxes. Mini
// games call SetHowTo / ShowResponse. The HUD hides itself while the scoreboard
// (T) is open, and after 15 seconds of no activity.
public class MiniGameHUD : MonoBehaviour
{
    public static MiniGameHUD Instance;

    [Header("Panel to show/hide")]
    public GameObject hudPanel;

    [Header("Assign your TMP text boxes")]
    [SerializeField] TMP_Text howToText;    // how to play
    [SerializeField] TMP_Text responseText; // transient messages ("Too long")
    [SerializeField] TMP_Text scoreText;    // current player score

    [SerializeField] float responseSeconds = 2f;
    [SerializeField] float hideAfterSeconds = 15f; // hide if no activity this long

    Coroutine responseRoutine;
    float lastActivity;
    bool playing;          // true while a mini-game wants the HUD up
    bool scoreboardWasOpen; // tracks the scoreboard open/close edge

    void Awake()
    {
        Instance = this;
        if (hudPanel != null) hudPanel.SetActive(false);
    }

    void OnDestroy()
    {
        if (Instance == this) Instance = null;
    }

    void OnEnable()
    {
        if (ScoreManager.Instance != null)
            ScoreManager.Instance.OnScoreChanged += HandleScoreChanged;
    }

    void OnDisable()
    {
        if (ScoreManager.Instance != null)
            ScoreManager.Instance.OnScoreChanged -= HandleScoreChanged;
    }

    void Update()
    {
        if (!playing) return;

        bool scoreboardOpen = Scoreboard.Instance != null && Scoreboard.Instance.isVisible;

        // while the scoreboard is open: hide the HUD, but freeze the idle timer so
        // checking the score doesn't count against you. the game "remembers".
        if (scoreboardOpen)
        {
            if (hudPanel != null) hudPanel.SetActive(false);
            lastActivity = Time.time;
            scoreboardWasOpen = true;
            return;
        }

        // scoreboard just closed -> bring the HUD back with a fresh 15s
        if (scoreboardWasOpen)
        {
            scoreboardWasOpen = false;
            if (hudPanel != null) hudPanel.SetActive(true);
            lastActivity = Time.time;
        }

        // stopped playing for 15s -> hide
        if (Time.time - lastActivity > hideAfterSeconds)
        {
            Hide();
            return;
        }

        if (hudPanel != null) hudPanel.SetActive(true);
    }

    // bring the HUD up and reset the inactivity timer (called on any game action)
    public void Show()
    {
        playing = true;
        lastActivity = Time.time;
        if (hudPanel != null) hudPanel.SetActive(true);
        UpdateScore();
    }

    public void Hide()
    {
        playing = false;
        if (hudPanel != null) hudPanel.SetActive(false);
    }

    // how-to-play text for the current mini-game
    public void SetHowTo(string text)
    {
        Show();
        if (howToText != null) howToText.text = text;
    }

    // transient response message; clears itself after a few seconds
    public void ShowResponse(string text)
    {
        ShowResponse(text, responseSeconds);
    }

    public void ShowResponse(string text, float seconds)
    {
        Show();
        if (responseText == null) return;
        responseText.text = text;
        if (responseRoutine != null) StopCoroutine(responseRoutine);
        responseRoutine = StartCoroutine(ClearResponseAfter(seconds));
    }

    IEnumerator ClearResponseAfter(float seconds)
    {
        yield return new WaitForSeconds(seconds);
        if (responseText != null) responseText.text = "";
    }

    // set the score box yourself (otherwise it auto-updates from ScoreManager)
    public void SetScore(string text)
    {
        if (scoreText != null) scoreText.text = text;
    }

    public void SetScore(int points)
    {
        SetScore("Score: " + points);
    }

    void HandleScoreChanged(Player player, int score)
    {
        if (player != PhotonNetwork.LocalPlayer) return;
        lastActivity = Time.time; // scoring counts as playing
        UpdateScore();
    }

    void UpdateScore()
    {
        if (scoreText != null && ScoreManager.Instance != null)
            scoreText.text = "Score: " + ScoreManager.Instance.GetScore();
    }
}
