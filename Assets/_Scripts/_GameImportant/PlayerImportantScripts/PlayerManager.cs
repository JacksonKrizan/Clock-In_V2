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

    public override void OnEnable()
    {
        base.OnEnable();
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    public override void OnDisable()
    {
        base.OnDisable();
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        // Don't spawn if we are in the Menu
        if (scene.name == "_Menu" || scene.name == "Menu") return;

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
        else if (sceneName == "Chef")
        {
            prefabName = "FirePlayerController";
        }
        else if (sceneName == "GameDev")
        {
            prefabName = "GamingPlayerController";
        }

        //Debug.Log($"[PlayerManager] Map detected: {sceneName}. Choosing prefab: {prefabName}");

        Vector3 spawnPos = Vector3.zero;
        Quaternion spawnRot = Quaternion.identity;
        Transform spawnpoint = SpawnManager.Instance.GetSpawnpoint();
        controller = PhotonNetwork.Instantiate(Path.Combine("PhotonPrefabs/", prefabName), spawnPos, spawnRot, 0, new object[] { PV.ViewID });
    }

    public static PlayerManager Find(Player player)
    {
        return FindObjectsOfType<PlayerManager>().SingleOrDefault(x => x.PV.Owner == player);
    }
}
