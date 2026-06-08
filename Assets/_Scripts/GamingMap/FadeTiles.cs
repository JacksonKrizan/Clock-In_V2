using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FadeTiles : MonoBehaviour
{

    public Tiles[] floorTiles; 

    private void Awake()
    {

        floorTiles = GetComponentsInChildren<Tiles>();


        Debug.Log("Found " + floorTiles.Length + " tiles.");

 
        foreach (Tiles tile in floorTiles)
        {
            Debug.Log("Found tile: " + tile.name);
        }
    }
}