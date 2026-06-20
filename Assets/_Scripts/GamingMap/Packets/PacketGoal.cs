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
            // OnCollisionEnter fires on every client; only the packet owner awards,
            // so one delivery counts once (credited to that owner's player).
            PhotonView packetPV = collision.gameObject.GetComponent<PhotonView>();
            if (packetPV != null && packetPV.IsMine && ScoreManager.Instance != null)
            {
                ScoreManager.Instance.AddScore(pointsPerDelivery);
            }
        }
    }
}
