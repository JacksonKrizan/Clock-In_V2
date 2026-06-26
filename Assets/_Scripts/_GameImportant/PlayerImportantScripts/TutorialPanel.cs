using UnityEngine;
using TMPro;

// Quick "how to play" panel shown the moment you join a world. Put this on an
// always-active object (e.g. the Canvas) and drag in the panel it should show.
// The cursor is freed while it's up so the player can read/click, and Q closes
// it and re-locks the cursor back to play. Edit the message per world in the
// Inspector (controls differ between maps).
public class TutorialPanel : MonoBehaviour
{
    [Header("Panel to show on join")]
    public GameObject tutorialPanel;

    [Header("Optional - filled with the text below")]
    [SerializeField] TMP_Text tutorialText;

    [TextArea(4, 10)]
    [SerializeField] string message =
        "HOW TO PLAY\n\n" +
        "WASD - move\n" +
        "Mouse - look around\n" +
        "Left Click - pick up / drop\n" +
        "Right Click - spray water\n" +
        "T - scoreboard\n" +
        "Esc - quit the game\n\n" +
        "Put out the fires and keep the buildings safe!\n\n" +
        "Press Q to close";

    [SerializeField] KeyCode closeKey = KeyCode.Q;

    bool isOpen;

    void Start()
    {
        Open();
    }

    void Update()
    {
        if (isOpen && Input.GetKeyDown(closeKey))
            Close();
    }

    public void Open()
    {
        isOpen = true;
        if (tutorialText != null) tutorialText.text = message;
        if (tutorialPanel != null) tutorialPanel.SetActive(true);

        // free the cursor so the player can read / click while it's up
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    public void Close()
    {
        isOpen = false;
        if (tutorialPanel != null) tutorialPanel.SetActive(false);

        // back to playing - lock the cursor again
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }
}
