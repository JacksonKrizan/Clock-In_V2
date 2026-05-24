using UnityEngine;
using Photon.Pun;

public class PacketGoal : MonoBehaviour
{
    public string packetTag = "Packet";

    void OnTriggerEnter(Collider other)
    {
        // Check if the object entering the trigger is a Packet
        if (other.CompareTag(packetTag))
        {
            Packet packet = other.GetComponent<Packet>();
            if (packet != null)
            {
                // Trigger the delivery across the network
                packet.photonView.RPC("Deliver", RpcTarget.AllBuffered);
                Debug.Log("Packet reached the delivery goal!");
            }
        }
    }
}
