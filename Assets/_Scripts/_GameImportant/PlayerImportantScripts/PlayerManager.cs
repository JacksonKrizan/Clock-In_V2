using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using System.IO;
using System.Linq;
using Photon.Realtime;
using Hashtable = ExitGames.Client.Photon.Hashtable;
using UnityEngine.SceneManagement;



public class PlayerManager : MonoBehaviourPunCallbacks
{
    PhotonView PV;
    //PhotonView PV;
    GameObject controller;
    [SerializeField] int mapToPlayer;

    private void Awake()
    {
        PV = GetComponent<PhotonView>();
    }

    void Start()
    {
        if (PV.IsMine)
        {
            CreateController(SceneManager.GetActiveScene());
        }
    }

    void CreateController(Scene scene)
    {
        //PhotonNetwork.Instantiate(Path.Combine("PhotonPrefabs", "PlayerManager"), Vector3.zero, Quaternion.identity);
                    //controller = PhotonNetwork.Instantiate(Path.Combine("PhotonPrefabs", "GamingPlayerController"), Vector3.zero, Quaternion.identity);

        mapToPlayer = scene.buildIndex;
        if (mapToPlayer == 1)
        {
            Transform spawnpoint = SpawnManager.Instance.GetSpawnpoint();
            controller = PhotonNetwork.Instantiate(Path.Combine("PhotonPrefabs", "GamingPlayerController"), spawnpoint.position, spawnpoint.rotation, 0, new object[] { PV.ViewID });
        } 
        else if (mapToPlayer == 2)
        {
            Transform spawnpoint = SpawnManager.Instance.GetSpawnpoint();
            controller = PhotonNetwork.Instantiate(Path.Combine("PhotonPrefabs", "FirePlayerController"), spawnpoint.position, spawnpoint.rotation, 0, new object[] { PV.ViewID });
        }
        else if (mapToPlayer == 3)
        {
            Transform spawnpoint = SpawnManager.Instance.GetSpawnpoint();
            controller = PhotonNetwork.Instantiate(Path.Combine("PhotonPrefabs", "AutoPlayerController"), spawnpoint.position, spawnpoint.rotation, 0, new object[] { PV.ViewID });
        }else
        {
            Transform spawnpoint = SpawnManager.Instance.GetSpawnpoint();
            controller = PhotonNetwork.Instantiate(Path.Combine("PhotonPrefabs", "PlayerController"), spawnpoint.position, spawnpoint.rotation, 0, new object[] { PV.ViewID });
        }





        //Debug.Log("Creating player controller for " + PV.Owner.NickName);
        //Transform spawnpoint = SpawnManager.Instance.GetSpawnpoint();
        //controller = PhotonNetwork.Instantiate("PhotonPrefabs/PlayerController", spawnpoint.position, spawnpoint.rotation, 0, new object[] { PV.ViewID });
    }
    
    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        // Debug.Log("New Player Joined: " + newPlayer.NickName);
        // Syncing logic can be added here if needed, but the previous RPC call was to a non-existent method.
    }

    public static PlayerManager Find(Player player)
    {
        // Optimization: FindObjectsOfType is expensive. 
        // Consider using a static Dictionary<Player, PlayerManager> to cache these.
        return FindObjectsOfType<PlayerManager>().SingleOrDefault(x => x.PV.Owner == player);
    }

}
