#if FIREBASE_ENABLED
using UnityEngine;
using TMPro;

/// <summary>
/// Wires the Auth panel UI to <see cref="AuthManager"/>. Assign the input fields and
/// status text in the Inspector, and hook the three public methods to your button
/// OnClick events.
/// </summary>
public class AuthMenuUI : MonoBehaviour
{
    [SerializeField] private TMP_InputField emailInput;
    [SerializeField] private TMP_InputField passwordInput;
    [SerializeField] private TMP_InputField displayNameInput;
    [SerializeField] private TMP_Text statusText;
    [SerializeField] private string menuToOpenOnSuccess = "title";

    public void OnSignUpClicked()
    {
        SetStatus("Creating account...");
        AuthManager.Instance.SignUp(emailInput.text, passwordInput.text,
            displayNameInput != null ? displayNameInput.text : null, OnAuthResult);
    }

    public void OnSignInClicked()
    {
        SetStatus("Signing in...");
        AuthManager.Instance.SignIn(emailInput.text, passwordInput.text, OnAuthResult);
    }

    public void OnGuestClicked()
    {
        AuthManager.Instance.ContinueAsGuest(displayNameInput != null ? displayNameInput.text : null);
        OnAuthResult(true, null);
    }

    private void OnAuthResult(bool success, string error)
    {
        if (success)
        {
            SetStatus("");
            if (MenuManager.Instance != null && !string.IsNullOrEmpty(menuToOpenOnSuccess))
                MenuManager.Instance.OpenMenu(menuToOpenOnSuccess);
        }
        else
        {
            SetStatus("Error: " + error);
        }
    }

    private void SetStatus(string msg)
    {
        if (statusText != null) statusText.text = msg;
    }
}
#endif
