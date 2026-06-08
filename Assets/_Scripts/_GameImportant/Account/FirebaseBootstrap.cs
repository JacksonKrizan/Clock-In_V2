#if FIREBASE_ENABLED
using System;
using UnityEngine;
using Firebase;
using Firebase.Extensions;

/// <summary>
/// Initializes the Firebase SDK once at startup and reports readiness. Lives across
/// scenes. AuthManager and LeaderboardManager wait on <see cref="IsReady"/> /
/// <see cref="OnReady"/> before touching Firebase.
///
/// Activated only when the FIREBASE_ENABLED scripting define symbol is set (add it in
/// Project Settings > Player > Scripting Define Symbols after importing the SDK).
/// </summary>
public class FirebaseBootstrap : MonoBehaviour
{
    public static FirebaseBootstrap Instance { get; private set; }
    public static bool IsReady { get; private set; }

    /// <summary>Fired once dependencies are resolved.</summary>
    public event Action OnReady;

    private void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(gameObject); return; }
        Instance = this;
        DontDestroyOnLoad(gameObject);

        FirebaseApp.CheckAndFixDependenciesAsync().ContinueWithOnMainThread(task =>
        {
            if (task.Result == DependencyStatus.Available)
            {
                IsReady = true;
                Debug.Log("[FirebaseBootstrap] Firebase ready.");
                OnReady?.Invoke();
            }
            else
            {
                Debug.LogError($"[FirebaseBootstrap] Could not resolve Firebase dependencies: {task.Result}");
            }
        });
    }
}
#endif
