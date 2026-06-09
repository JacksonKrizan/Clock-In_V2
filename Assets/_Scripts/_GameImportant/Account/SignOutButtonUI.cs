#if FIREBASE_ENABLED
using UnityEngine;

/// <summary>
/// Drop this on the title menu (or its Sign Out button) and hook the button's OnClick to
/// OnSignOutClicked. It signs the player out, clears their running score, and returns to
/// the login screen. The player stays connected to Photon, so they can sign back in.
/// </summary>
public class SignOutButtonUI : MonoBehaviour
{
    public void OnSignOutClicked()
    {
        if (AuthManager.Instance != null) AuthManager.Instance.SignOut();
        if (ScoreManager.Instance != null) ScoreManager.Instance.ResetScore();
        if (MenuManager.Instance != null) MenuManager.Instance.OpenMenu("login");
    }
}
#endif
