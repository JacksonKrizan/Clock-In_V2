using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine.SceneManagement;
using Unity.VisualScripting;
using UnityEditor.Build;

public class PlayerSettings : MonoBehaviourPunCallbacks
{
    [SerializeField] GameObject settingsMenu;
    /*public bool isSettingsMenuOpen;
    public override void OnLeftRoom()
    {
        PhotonNetwork.LeaveRoom();
        SceneManager.LoadScene(0);
    }
    public void ToggleSettingsMenu()
    {
        settingsMenu.SetActive(true);
        //settingsMenu.SetActive(!settingsMenu.activeSelf);
        isSettingsMenuOpen = !isSettingsMenuOpen;

    }
    public void CloseSettingsMenu()
    {
        settingsMenu.SetActive(false);
        isSettingsMenuOpen = false;
    }*/
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            settingsMenu.SetActive(!settingsMenu.activeSelf);
        }
    }

}
