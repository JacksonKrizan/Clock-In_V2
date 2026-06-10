using UnityEngine;

/// <summary>
/// Shows/hides the live scoreboard panel when the player presses a key (default: T).
///
/// IMPORTANT: put this on an object that stays ACTIVE during play (NOT on the scoreboard
/// panel itself, since a hidden object can't listen for the key). Assign the scoreboard
/// panel to <see cref="scoreboardPanel"/>.
/// </summary>
public class ScoreboardToggle : MonoBehaviour
{
    [SerializeField] private GameObject scoreboardPanel;
    [SerializeField] private KeyCode toggleKey = KeyCode.T;
    [SerializeField] private bool startHidden = true;

    private void Start()
    {
        if (scoreboardPanel != null)
            scoreboardPanel.SetActive(!startHidden);
    }

    private void Update()
    {
        if (scoreboardPanel == null) return;

        if (Input.GetKeyDown(toggleKey))
        {
            bool show = !scoreboardPanel.activeSelf;
            scoreboardPanel.SetActive(show);

            // Rebuild the list each time it opens so it's current.
            if (show)
            {
                LiveScoreboardUI board = scoreboardPanel.GetComponent<LiveScoreboardUI>();
                if (board == null) board = scoreboardPanel.GetComponentInChildren<LiveScoreboardUI>(true);
                if (board != null) board.Refresh();
            }
        }
    }
}
