using UnityEngine;
using Photon.Pun;

public class PacketGoal : MonoBehaviour
{
    public string packetTag = "Packages";
    [SerializeField] int pointsPerDelivery = 10;

    void OnCollisionEnter(Collision collision)
    {
        // Check if the object entering the trigger is a Packet
        if (collision.gameObject.CompareTag(packetTag))
        {
            //Packet packet = other.GetComponent<Packet>();
            Debug.Log("Goal");

            // Award points to the local player. AddScore can be called from anywhere;
            // this is the first concrete caller. NOTE: in multiplayer OnCollisionEnter
            // fires on every client, so you may later want to gate this to the packet's
            // owner so a single delivery isn't counted on each client.
            if (ScoreManager.Instance != null)
                ScoreManager.Instance.AddScore(pointsPerDelivery);
        }
    }
}
