using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine.SceneManagement;
using Unity.VisualScripting;
//using UnityEditor.Build;
using System.Security.Cryptography.X509Certificates;

public class PlayerSettings : MonoBehaviourPunCallbacks
{
    [SerializeField] GameObject settingsMenu;
    [SerializeField] GameObject howToPlayMenu;
    public bool isSettingsMenuOpen = false;

    // in-game Leave button (RoomManager loads the menu after the leave)
    public void LeaveGame()
    {
        PhotonNetwork.LeaveRoom();
    }

    void Update()
    {
        // A-003: Escape quits
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            QuitGame();
        }
    }

    public void QuitGame()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false; // Quit() no-ops in Editor
#else
        Application.Quit();
#endif
    }

    public void OpenMenuAndUnlockCursor()
    {
        settingsMenu.SetActive(true);
        isSettingsMenuOpen = true;

        Cursor.lockState = CursorLockMode.None;

        Cursor.visible = true;
    }

    public void LockCursorAndCloseMenu()
    {
        settingsMenu.SetActive(false);
        isSettingsMenuOpen = false;
        
        Cursor.lockState = CursorLockMode.Locked;

        Cursor.visible = false;
    }
    public void OpenHowToPlayMenu()
    {
        howToPlayMenu.SetActive(!howToPlayMenu.activeSelf);
    }
    

}
