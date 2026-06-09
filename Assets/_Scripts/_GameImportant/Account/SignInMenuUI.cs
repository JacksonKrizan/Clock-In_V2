#if FIREBASE_ENABLED
using UnityEngine;
using TMPro;

/// <summary>
/// The "signin" menu: logs into an EXISTING account only. Firebase rejects a wrong
/// password or an email with no account, and we show that in plain English. On success
/// the player's saved gamertag becomes their in-game name and we go to the title screen.
///
/// Hook the Log In button to OnSignInClicked and the Back button to OnBackClicked.
/// </summary>
public class SignInMenuUI : MonoBehaviour
{
    [SerializeField] private TMP_InputField emailInput;
    [SerializeField] private TMP_InputField passwordInput;
    [SerializeField] private TMP_Text statusText;

    public void OnSignInClicked()
    {
        if (AuthManager.Instance == null) { SetStatus("Auth not ready."); return; }

        string email = emailInput != null ? emailInput.text.Trim() : "";
        string password = passwordInput != null ? passwordInput.text : "";

        if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(password))
        {
            SetStatus("Enter your email and password.");
            return;
        }

        SetStatus("Signing in...");
        AuthManager.Instance.SignIn(email, password, (success, error) =>
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
