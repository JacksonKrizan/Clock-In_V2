using UnityEngine;
using Photon.Pun;

// Keeps the room stocked with hoses: one per player, plus one spare. Only the
// master client spawns (so everyone shares the same hoses), and because it just
// tops the count back up, a hose that falls out of the world and destroys itself
// gets replaced automatically.
public class HoseSpawner : MonoBehaviour
{
    [Tooltip("Prefab name relative to a Resources folder, e.g. PhotonPrefabs/FireHose")]
    [SerializeField] string hosePrefabName = "PhotonPrefabs/FireHose";
    [SerializeField] Vector3 spawnPosition;
    [Tooltip("Spread hoses out so they don't stack on the same spot")]
    [SerializeField] float spawnSpacing = 1.5f;

    void Update()
    {
        // hoses are networked, so we need a room; only the master tops them up
        if (!PhotonNetwork.IsConnected) return;
        if (!PhotonNetwork.IsMasterClient) return;

        int target = PhotonNetwork.CurrentRoom.PlayerCount + 1; // one each, plus a spare
        if (CountHoses() < target)
        {
            // offset each new hose a little so they don't all land on one point
            Vector3 pos = spawnPosition + Vector3.right * (CountHoses() * spawnSpacing);
            PhotonNetwork.Instantiate(hosePrefabName, pos, Quaternion.identity);
        }
    }

    int CountHoses()
    {
        return FindObjectsOfType<HoseControl>().Length;
    }
}
