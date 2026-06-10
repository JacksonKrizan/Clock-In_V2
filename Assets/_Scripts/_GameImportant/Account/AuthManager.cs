#if FIREBASE_ENABLED
using System;
using UnityEngine;
using Photon.Pun;
using Firebase;
using Firebase.Auth;
using Firebase.Extensions;

/// <summary>
/// Handles email/password sign-up &amp; sign-in (saved accounts) and guest mode
/// (local only, never written to the leaderboard). On success the chosen display name
/// becomes the Photon NickName and feeds the existing room/lobby flow unchanged.
/// </summary>
public class AuthManager : MonoBehaviour
{
    public static AuthManager Instance { get; private set; }

    private FirebaseAuth auth;

    public bool IsSignedIn => auth != null && auth.CurrentUser != null;
    public bool IsGuest { get; private set; }
    public string CurrentUid => IsSignedIn ? auth.CurrentUser.UserId : null;
    public string DisplayName { get; private set; }

    private void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(gameObject); return; }
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    private void Start()
    {
        if (FirebaseBootstrap.IsReady) Init();
        else if (FirebaseBootstrap.Instance != null) FirebaseBootstrap.Instance.OnReady += Init;
    }

    private void Init()
    {
        if (auth != null) return; // guard against OnReady firing after an initial IsReady hit
        auth = FirebaseAuth.DefaultInstance;
    }

    /// <summary>Create a new saved account, then sign in.</summary>
    public void SignUp(string email, string password, string displayName, Action<bool, string> onComplete)
    {
        if (auth == null) { onComplete?.Invoke(false, "Firebase not ready yet."); return; }

        auth.CreateUserWithEmailAndPasswordAsync(email, password).ContinueWithOnMainThread(task =>
        {
            if (task.IsCanceled || task.IsFaulted)
            {
                onComplete?.Invoke(false, FriendlyError(task.Exception));
                return;
            }

            FirebaseUser user = task.Result.User;
            string name = string.IsNullOrWhiteSpace(displayName) ? email.Split('@')[0] : displayName;

            user.UpdateUserProfileAsync(new UserProfile { DisplayName = name })
                .ContinueWithOnMainThread(_ =>
                {
                    ApplySignIn(name, false);
                    onComplete?.Invoke(true, null);
                });
        });
    }

    /// <summary>Sign in to an existing saved account.</summary>
    public void SignIn(string email, string password, Action<bool, string> onComplete)
    {
        if (auth == null) { onComplete?.Invoke(false, "Firebase not ready yet."); return; }

        auth.SignInWithEmailAndPasswordAsync(email, password).ContinueWithOnMainThread(task =>
        {
            if (task.IsCanceled || task.IsFaulted)
            {
                onComplete?.Invoke(false, FriendlyError(task.Exception));
                return;
            }

            FirebaseUser user = task.Result.User;
            string name = string.IsNullOrWhiteSpace(user.DisplayName) ? email.Split('@')[0] : user.DisplayName;
            ApplySignIn(name, false);
            onComplete?.Invoke(true, null);
        });
    }

    /// <summary>Play without an account. Scores are NOT written to the leaderboard.</summary>
    public void ContinueAsGuest(string displayName)
    {
        // Drop any real Firebase session first, otherwise CurrentUser (and CurrentUid)
        // would still report the previous signed-in account while IsGuest is true,
        // letting a "guest" leak a real uid to the leaderboard.
        if (auth != null && auth.CurrentUser != null) auth.SignOut();

        string name = string.IsNullOrWhiteSpace(displayName)
            ? "Guest" + UnityEngine.Random.Range(0, 10000).ToString("0000")
            : displayName;
        ApplySignIn(name, true);
    }

    /// <summary>Sign out of the current account (or end a guest session) and clear state.</summary>
    public void SignOut()
    {
        if (auth != null && auth.CurrentUser != null) auth.SignOut();
        IsGuest = false;
        DisplayName = null;
        PhotonNetwork.NickName = "";
        PlayerPrefs.DeleteKey("username");
        Debug.Log("[AuthManager] Signed out.");
    }

    private void ApplySignIn(string name, bool guest)
    {
        IsGuest = guest;
        DisplayName = name;
        PhotonNetwork.NickName = name;
        PlayerPrefs.SetString("username", name);
        Debug.Log($"[AuthManager] Signed in as '{name}' (guest={guest}).");
    }

    private static string FriendlyError(AggregateException ex)
    {
        if (ex == null) return "Something went wrong. Try again.";
        foreach (Exception inner in ex.Flatten().InnerExceptions)
        {
            if (inner is FirebaseException fe)
            {
                switch ((AuthError)fe.ErrorCode)
                {
                    case AuthError.UserNotFound:
                        return "No account found for that email. Try Sign Up.";
                    case AuthError.WrongPassword:
                        return "Wrong password.";
                    case AuthError.InvalidCredential:   // newer SDKs lump wrong email/password here
                        return "Wrong email or password.";
                    case AuthError.InvalidEmail:
                        return "That doesn't look like a valid email.";
                    case AuthError.MissingEmail:
                        return "Enter an email.";
                    case AuthError.MissingPassword:
                        return "Enter a password.";
                    case AuthError.EmailAlreadyInUse:
                        return "That email is already registered. Try Sign In.";
                    case AuthError.WeakPassword:
                        return "Password is too weak (use at least 6 characters).";
                    case AuthError.TooManyRequests:
                        return "Too many attempts. Wait a bit and try again.";
                    case AuthError.NetworkRequestFailed:
                        return "No internet connection.";
                    default:
                        return "Sign-in failed: " + (AuthError)fe.ErrorCode;
                }
            }
        }
        return ex.Message;
    }
}
#endif
