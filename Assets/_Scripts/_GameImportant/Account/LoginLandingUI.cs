#if FIREBASE_ENABLED
using UnityEngine;
using TMPro;

/// <summary>
/// The first screen the player sees (menu name "login"). Three buttons:
///   - Sign In  -> opens the "signin" menu
///   - Sign Up  -> opens the "signup" menu
///   - Guest    -> picks a local name and goes straight to "title" (never saved online)
///
/// Put this on the login landing panel and hook the three buttons to the matching
/// methods below. The optional guestNameInput lets a guest choose a display name.
/// </summary>
public class LoginLandingUI : MonoBehaviour
{
    [Tooltip("Optional: a name field for guests. Leave unassigned to auto-generate 'Guest####'.")]
    [SerializeField] private TMP_InputField guestNameInput;

    public void OnSignInMenuClicked()
    {
        if (MenuManager.Instance != null) MenuManager.Instance.OpenMenu("signin");
    }

    public void OnSignUpMenuClicked()
    {
        if (MenuManager.Instance != null) MenuManager.Instance.OpenMenu("signup");
    }

    public void OnGuestClicked()
    {
        if (AuthManager.Instance == null)
        {
            Debug.LogError("[LoginLandingUI] AuthManager missing from scene.");
            return;
        }
        string tag = guestNameInput != null ? guestNameInput.text : null;
        AuthManager.Instance.ContinueAsGuest(tag);
        if (MenuManager.Instance != null) MenuManager.Instance.OpenMenu("title");
    }
}
#endif
