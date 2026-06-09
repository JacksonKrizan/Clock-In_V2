#if FIREBASE_ENABLED
using UnityEngine;
using TMPro;

/// <summary>
/// The "signup" menu: creates a NEW account (email + password) and a gamertag. The
/// gamertag is stored on the account, used as the in-game name, and saved next to the
/// player's leaderboard score. On success we go to the title screen.
///
/// Hook the Create Account button to OnSignUpClicked and the Back button to OnBackClicked.
/// </summary>
public class SignUpMenuUI : MonoBehaviour
{
    [SerializeField] private TMP_InputField emailInput;
    [SerializeField] private TMP_InputField passwordInput;
    [SerializeField] private TMP_InputField gamerTagInput;
    [SerializeField] private TMP_Text statusText;

    public void OnSignUpClicked()
    {
        if (AuthManager.Instance == null) { SetStatus("Auth not ready."); return; }

        string email = emailInput != null ? emailInput.text.Trim() : "";
        string password = passwordInput != null ? passwordInput.text : "";
        string gamerTag = gamerTagInput != null ? gamerTagInput.text.Trim() : "";

        if (string.IsNullOrEmpty(gamerTag)) { SetStatus("Pick a gamertag."); return; }
        if (string.IsNullOrEmpty(email)) { SetStatus("Enter an email."); return; }
        if (password.Length < 6) { SetStatus("Password must be at least 6 characters."); return; }

        SetStatus("Creating account...");
        AuthManager.Instance.SignUp(email, password, gamerTag, (success, error) =>
        {
            if (success)
            {
                SetStatus("");
                if (MenuManager.Instance != null) MenuManager.Instance.OpenMenu("title");
            }
            else
            {
                SetStatus(error);
            }
        });
    }

    public void OnBackClicked()
    {
        SetStatus("");
        if (MenuManager.Instance != null) MenuManager.Instance.OpenMenu("login");
    }

    private void SetStatus(string msg)
    {
        if (statusText != null) statusText.text = msg;
    }
}
#endif
