using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PacketSpawner : MonoBehaviour
{

    [SerializeField] GameObject packetToSpawn;
    [SerializeField] Vector3 spawnPosition;
    [SerializeField] int maxPackets = 3;
    public int currentPacketCount = 0;

    [Header("HUD")]
    [TextArea][SerializeField] string howTo = "Pick up a packet and carry it to the green goal.";


    void Start()
    {

    }

    // show the HUD (called when a packet is first picked up)
    public void ShowHud()
    {
        if (MiniGameHUD.Instance != null) MiniGameHUD.Instance.SetHowTo(howTo); // also shows the HUD
    }

    // Update is called once per frame
    void Update()
    {
        if (currentPacketCount < maxPackets)
        {
            SpawnObject();
            currentPacketCount++;
        }
        
    }
    void SpawnObject()
    {
        Quaternion spawnRotation = Quaternion.identity;
        Instantiate(packetToSpawn, spawnPosition, spawnRotation);
    }
}
