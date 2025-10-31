using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using UnityEngine.SceneManagement;
using System.IO;
public class RoomManager : MonoBehaviourPunCallbacks
{
    public static RoomManager Instance;
    //public Launcher mapNumber;
    
    private void Awake()
    {
        if (Instance)
        {
            Destroy(gameObject);
            return;
        }
        DontDestroyOnLoad(gameObject);
        Instance = this;
    }

    public override void OnEnable()
    {
        base.OnEnable();
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode loadSceneMode)
    {
        if (scene.buildIndex != 0) // Don't spawn in menu (scene 0)
        {
            // Use forward slashes for Photon resource paths
            string playerManagerPath = "PhotonPrefabs/PlayerManager";
            
            // Verify the prefab exists before trying to instantiate
            if (Resources.Load(playerManagerPath) != null)
            {
                PhotonNetwork.Instantiate(playerManagerPath, Vector3.zero, Quaternion.identity);
                Debug.Log($"PlayerManager instantiated in scene {scene.buildIndex}");
            }
            else
            {
                Debug.LogError($"Failed to find PlayerManager prefab at Resources/{playerManagerPath}");
            }
        }
    }

}
