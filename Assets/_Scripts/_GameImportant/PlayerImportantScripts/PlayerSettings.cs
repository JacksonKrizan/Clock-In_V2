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

    public override void OnLeftRoom()
    {
        PhotonNetwork.LeaveRoom();
        SceneManager.LoadScene(0);
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            OpenMenuAndUnlockCursor();
        }
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
