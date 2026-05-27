using UnityEngine;
using Photon.Pun;

public class PacketGoal : MonoBehaviour
{
    public string packetTag = "Packages";

    void OnCollisionEnter(Collision collision)
    {
        // Check if the object entering the trigger is a Packet
        if (collision.gameObject.CompareTag(packetTag))
        {
            //Packet packet = other.GetComponent<Packet>();
            Debug.Log("Goal");
        }
    }
}
