using UnityEngine;
using Photon.Pun;

public class PacketGoal : MonoBehaviour
{
    public string packetTag = "Packages";
    [SerializeField] int pointsPerDelivery = 10;

    void OnCollisionEnter(Collision collision)
    {
        if (!collision.gameObject.CompareTag(packetTag)) return;

        // Packets are spawned locally per client, so this delivery only happens on
        // this client - no IsMine gate needed. Score once, on the first delivery.
        Packet packet = collision.gameObject.GetComponent<Packet>();
        bool fresh = packet == null || packet.OnDelivered();

        if (fresh && ScoreManager.Instance != null)
            ScoreManager.Instance.AddScore(pointsPerDelivery);
    }
}
