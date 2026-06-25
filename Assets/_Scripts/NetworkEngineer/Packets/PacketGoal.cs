using UnityEngine;
using Photon.Pun;

public class PacketGoal : MonoBehaviour
{
    public string packetTag = "Packages";
    [SerializeField] int pointsPerDelivery = 10;

    void OnCollisionEnter(Collision collision)
    {
        if (!collision.gameObject.CompareTag(packetTag)) return;

        Packet packet = collision.gameObject.GetComponent<Packet>();
        if (packet == null) return;

        // packets are networked now: only the owner (the carrier) delivers, so it
        // happens once and the carrier gets the points. Delivered state syncs to
        // everyone via the packet's PhotonView.
        if (PhotonNetwork.IsConnected && !packet.photonView.IsMine) return;

        if (packet.OnDelivered() && ScoreManager.Instance != null)
            ScoreManager.Instance.AddScore(pointsPerDelivery);
    }
}
