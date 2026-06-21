using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PacketSpawner : MonoBehaviour
{

    [SerializeField] GameObject packetToSpawn;
    [SerializeField] Vector3 spawnPosition;
    [SerializeField] int maxPackets = 3;
    public int currentPacketCount = 0;
    

    
    void Start()
    {

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
