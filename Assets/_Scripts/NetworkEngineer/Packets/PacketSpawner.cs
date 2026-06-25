using UnityEngine;
using Photon.Pun;

public class PacketSpawner : MonoBehaviour
{
    [Tooltip("Prefab name relative to a Resources folder, e.g. PhotonPrefabs/Packet")]
    [SerializeField] string packetPrefabName = "PhotonPrefabs/Packet";
    [SerializeField] Vector3 spawnPosition;
    [SerializeField] int maxPackets = 3;

    [Header("HUD")]
    [TextArea][SerializeField] string howTo = "Pick up a packet and carry it to the green goal.";

    // show the HUD (called when a packet is first picked up)
    public void ShowHud()
    {
        if (MiniGameHUD.Instance != null) MiniGameHUD.Instance.SetHowTo(howTo);
    }

    void Update()
    {
        // only the master client spawns, so everyone shares the same packets
        if (PhotonNetwork.IsConnected && !PhotonNetwork.IsMasterClient) return;
        if (!PhotonNetwork.IsConnected) return; // packets are networked - need a room

        if (CountPackets() < maxPackets)
            PhotonNetwork.Instantiate(packetPrefabName, spawnPosition, Quaternion.identity);
    }

    // count the packets currently alive in the scene
    int CountPackets()
    {
        return FindObjectsOfType<Packet>().Length;
    }
}
