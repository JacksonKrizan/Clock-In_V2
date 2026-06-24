using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using System.IO;
using System.Linq;
using Photon.Realtime;
using UnityEngine.SceneManagement;

public class PlayerManager : MonoBehaviourPunCallbacks
{
    PhotonView PV;
    GameObject controller;

    private void Awake()
    {
        PV = GetComponent<PhotonView>();
    }

    void Start()
    {
        // RoomManager instantiates this PlayerManager AFTER the scene has finished
        // loading (it waits for the room to be ready first), so listening for
        // SceneManager.sceneLoaded would never fire for the current scene. Spawning
        // from Start() runs as soon as this object is created, which is what we want.

        // Safety net: never spawn a player in the menu.
        string sceneName = SceneManager.GetActiveScene().name;
        if (sceneName == "_Menu" || sceneName == "Menu") return;

        if (PV.IsMine)
        {
            CreateController();
        }
    }

    void CreateController()
    {
        string sceneName = SceneManager.GetActiveScene().name;
        string prefabName = "PlayerController"; // Default fallback

        // Select the specific prefab for this map based on actual file names
        if (sceneName == "AutoShop")
        {
            prefabName = "AutoPlayerController";
        }
        else if (sceneName == "Firemap")
        {
            prefabName = "FirePlayerController";
        }
        else if (sceneName == "GameDev")
        {
            prefabName = "GamingPlayerController";
        }

        //Debug.Log($"[PlayerManager] Map detected: {sceneName}. Choosing prefab: {prefabName}");

        // Use a spawn point if a SpawnManager exists in this scene; otherwise fall back
        // to the origin so a missing SpawnManager can't crash the spawn.
        Vector3 spawnPos = Vector3.zero;
        Quaternion spawnRot = Quaternion.identity;
        if (SpawnManager.Instance != null)
        {
            Transform spawnpoint = SpawnManager.Instance.GetSpawnpoint();
            spawnPos = spawnpoint.position;
            spawnRot = spawnpoint.rotation;
        }
        else
        {
            Debug.LogWarning("[PlayerManager] No SpawnManager in scene - spawning at origin.");
        }

        controller = PhotonNetwork.Instantiate(Path.Combine("PhotonPrefabs/", prefabName), spawnPos, spawnRot, 0, new object[] { PV.ViewID });
    }

    public static PlayerManager Find(Player player)
    {
        return FindObjectsOfType<PlayerManager>().SingleOrDefault(x => x.PV.Owner == player);
    }
}
