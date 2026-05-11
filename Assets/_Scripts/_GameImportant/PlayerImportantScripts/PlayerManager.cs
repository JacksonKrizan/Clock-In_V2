using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using System.IO;
using System.Linq;
using Photon.Realtime;
using Hashtable = ExitGames.Client.Photon.Hashtable;
using System.Numerics;



public class PlayerManager : MonoBehaviourPunCallbacks
{
    PhotonView PV;
    GameObject controller;

    Launcher launcher;
    string prefabPath = "PhotonPrefabs/PlayerController"; 
    private void Awake()
    {
        PV = GetComponent<PhotonView>();
    }

    void Start()
    {
        if (PV.IsMine)
        {
            CreateController();
        }
    }

    void CreateController()
    {
        GetPlayerController();
        /*if (launcher.mapNumber == 0)
        {
            //Debug.Log("Map number is 0, using GameDev prefab");
            prefabPath = "PhotonPrefabs/GamingPlayerController";
        }
        else
        {
            prefabPath = "PhotonPrefabs/PlayerController";
        }*/
        Debug.Log("Creating player controller for " + PV.Owner.NickName);
        Transform spawnpoint = SpawnManager.Instance.GetSpawnpoint();
        controller = PhotonNetwork.Instantiate(prefabPath, spawnpoint.position, spawnpoint.rotation, 0, new object[] { PV.ViewID });
    }
    void GetPlayerController()
    {
        if (launcher.mapNumber == 0)
        {
            //Debug.Log("Map number is 0, using GameDev prefab");
            prefabPath = "PhotonPrefabs/GamingPlayerController";
        }
        else if (launcher.mapNumber == 1)
        {
            prefabPath = "PhotonPrefabs/AutoPlayerController";
        }
        else if (launcher.mapNumber == 2)
        {
            prefabPath = "PhotonPrefabs/FirePlayerController";
        }
        else
        {
            prefabPath = "PhotonPrefabs/PlayerController";
        }
        
    }
    /*void CreateController()
    {
        //string prefabPath = "PhotonPrefabs/GamingPlayerController"; // Default path
        string prefabPath = "PhotonPrefabs/PlayerController"; // Default path
        
        Debug.Log("Creating player controller for " + PV.Owner.NickName);
        Transform spawnpoint = SpawnManager.Instance.GetSpawnpoint();
        if (launcher.mapNumber == 0)
        {
            //Debug.Log("Map number is 0, using GameDev prefab");
            prefabPath = "PhotonPrefabs/GamingPlayerController";
        }
        else if (launcher.mapNumber == 1)
        {
            prefabPath = "PhotonPrefabs/AutoPlayerController";
        }
        else if (launcher.mapNumber == 2)
        {
            prefabPath = "PhotonPrefabs/FirePlayerController";
        }

        controller = PhotonNetwork.Instantiate(prefabPath, spawnpoint.position, spawnpoint.rotation, 0, new object[] { PV.ViewID });
    }*/

    /*void CreateController()
    {
        string sceneName = UnityEngine.SceneManagement.SceneManager.GetActiveScene().name;
        string prefabPath = "PhotonPrefabs/PlayerController"; // Default path

        //if (sceneName == "GameDev")
        if (launcher.mapNumber == 0)
        {
            Debug.Log("Map number is 0, using GameDev prefab");
            prefabPath = "PhotonPrefabs/GamingPlayerController";
        }
        else if (sceneName == "WarehouseScene")
        {
            prefabPath = "PhotonPrefabs/WarehousePlayerController";
        }
        else if (sceneName == "FactoryScene")
        {
            prefabPath = "PhotonPrefabs/FactoryPlayerController";
        }
        Debug.Log("Creating player controller for " + PV.Owner.NickName + " using prefab: " + prefabPath);



        if (SpawnManager.Instance != null)
        {
            Transform spawnpoint = SpawnManager.Instance.GetSpawnpoint();
            controller = PhotonNetwork.Instantiate(prefabPath, spawnpoint.position, spawnpoint.rotation, 0, new object[] { PV.ViewID });
        }
        else
        {
            Debug.LogWarning("SpawnManager instance not found! Spawning at default position.");
        }
        
    }*/
    
    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        //Debug.Log("New Player Joined: " + newPlayer.NickName);

        
        if (PhotonNetwork.IsMasterClient)
        {
            foreach (GameObject player in GameObject.FindGameObjectsWithTag("Player"))
            {
                PhotonView pv = player.GetComponent<PhotonView>();
                pv.RPC("SyncExistingPlayer", newPlayer);
                Debug.Log("Syncing player for new player");
            }
        }
    }
    public static PlayerManager Find(Player player)
    {
        return FindObjectsOfType<PlayerManager>().SingleOrDefault(x => x.PV.Owner == player);
    }

}
