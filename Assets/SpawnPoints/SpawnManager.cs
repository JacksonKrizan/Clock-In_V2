using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices.WindowsRuntime;
using UnityEngine;

public class SpawnManager : MonoBehaviour
{
    public static SpawnManager Instance;

    Spawnpoint[] spawnpoints;

    private void Awake()
    {
        Instance = this;
        spawnpoints = GetComponentsInChildren<Spawnpoint>();
        Debug.Log("Found " + spawnpoints.Length + " spawnpoints.");
    }

    public Transform GetSpawnpoint()
    {
        // Check if we have any spawn points
        if (spawnpoints == null || spawnpoints.Length == 0)
        {
            Debug.LogWarning("No spawn points found! Make sure you have GameObjects with Spawnpoint component as children of SpawnManager");
            // Return the SpawnManager's transform as a fallback spawn position
            return transform;
        }

        // Get a random spawn point
        return spawnpoints[Random.Range(0, spawnpoints.Length)].transform;
    }
}
