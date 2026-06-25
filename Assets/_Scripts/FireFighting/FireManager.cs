using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;

// Randomly starts fires in the buildings. The master client decides which
// building lights up and when, then tells everyone via an RPC so all players see
// the same fires. Put this on a scene object that also has a PhotonView, and drag
// each building's Fire script into the list.
public class FireManager : MonoBehaviourPunCallbacks
{
    [Header("Buildings")]
    public List<Fire> buildingFires = new List<Fire>(); // drag each building's Fire here (up to 10)

    [Header("Randomness (tweak these)")]
    public float minTime = 5f;      // shortest wait before the next fire
    public float maxTime = 15f;     // longest wait before the next fire
    public int maxActiveFires = 3;  // how many can burn at the same time

    bool loopRunning;

    void Start()
    {
        // everyone starts with all buildings unlit (deterministic on every client)
        foreach (Fire f in buildingFires)
            if (f != null) f.currentIntensity = 0f;

        TryStartLoop();
    }

    // if the master client leaves, the new master takes over starting fires
    public override void OnMasterClientSwitched(Player newMaster)
    {
        TryStartLoop();
    }

    void TryStartLoop()
    {
        if (loopRunning) return;
        // the master client (or offline, for solo testing) runs the timer
        if (!PhotonNetwork.IsConnected || PhotonNetwork.IsMasterClient)
        {
            loopRunning = true;
            StartCoroutine(FireLoop());
        }
    }

    IEnumerator FireLoop()
    {
        while (true)
        {
            yield return new WaitForSeconds(Random.Range(minTime, maxTime));

            if (CountActiveFires() >= maxActiveFires) continue;

            int index = PickRandomOutFire();
            if (index >= 0) Ignite(index);
        }
    }

    void Ignite(int index)
    {
        if (index < 0 || index >= buildingFires.Count) return;
        // master sets the intensity; the Fire's own PhotonView syncs it to everyone
        // (this also means late joiners see fires that are already burning)
        if (buildingFires[index] != null)
            buildingFires[index].currentIntensity = 1f;
    }

    int CountActiveFires()
    {
        int count = 0;
        foreach (Fire f in buildingFires)
            if (f != null && f.currentIntensity > 0f) count++;
        return count;
    }

    // pick a random building that is currently out
    int PickRandomOutFire()
    {
        List<int> outFires = new List<int>();
        for (int i = 0; i < buildingFires.Count; i++)
            if (buildingFires[i] != null && buildingFires[i].currentIntensity <= 0f)
                outFires.Add(i);

        if (outFires.Count == 0) return -1;
        return outFires[Random.Range(0, outFires.Count)];
    }
}
