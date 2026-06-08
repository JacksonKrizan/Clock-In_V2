using UnityEngine;
using TMPro;

/// <summary>
/// Displays the live <see cref="ScoreManager"/> total in a TMP_Text. Drop this on a HUD
/// object in the gameplay scene and assign the text field in the Inspector.
/// </summary>
public class ScoreHUD : MonoBehaviour
{
    [SerializeField] private TMP_Text scoreText;
    [SerializeField] private string format = "Score: {0}";

    private void Start()
    {
        if (ScoreManager.Instance != null)
        {
            ScoreManager.Instance.OnScoreChanged += UpdateText;
            UpdateText(ScoreManager.Instance.CurrentScore);
        }
    }

    private void OnDestroy()
    {
        if (ScoreManager.Instance != null)
            ScoreManager.Instance.OnScoreChanged -= UpdateText;
    }

    private void UpdateText(int score)
    {
        if (scoreText != null)
            scoreText.text = string.Format(format, score);
    }
}
