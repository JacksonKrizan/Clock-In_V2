using System.Collections;
using UnityEngine;
using TMPro;
using Photon.Pun;
using Photon.Realtime;


public class MiniGameHUD : MonoBehaviour
{
    public static MiniGameHUD Instance;

    [Header("Assign your TMP text boxes")]
    [SerializeField] TMP_Text howToText;    // how to play
    [SerializeField] TMP_Text responseText; // transient messages ("Too long")
    [SerializeField] TMP_Text scoreText;    // current player score

    [SerializeField] float responseSeconds = 2f;

    Coroutine responseRoutine;

    void OnEnable()
    {
        Instance = this;
        if (ScoreManager.Instance != null)
        {
            ScoreManager.Instance.OnScoreChanged += HandleScoreChanged;
            UpdateScore();
        }
    }

    void OnDisable()
    {
        if (ScoreManager.Instance != null)
            ScoreManager.Instance.OnScoreChanged -= HandleScoreChanged;
        if (Instance == this) Instance = null;
    }

    // how-to-play text for the current mini-game
    public void SetHowTo(string text)
    {
        if (howToText != null) howToText.text = text;
    }

    // transient response message; clears itself after a few seconds
    public void ShowResponse(string text)
    {
        ShowResponse(text, responseSeconds);
    }

    public void ShowResponse(string text, float seconds)
    {
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
        if (player == PhotonNetwork.LocalPlayer) UpdateScore();
    }

    void UpdateScore()
    {
        if (scoreText != null && ScoreManager.Instance != null)
            scoreText.text = "Score: " + ScoreManager.Instance.GetScore();
    }
}
