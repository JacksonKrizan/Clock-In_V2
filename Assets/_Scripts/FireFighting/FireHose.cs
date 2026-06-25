using UnityEngine;
using Photon.Pun;

// The hose's water. The holder calls SetSpraying; it broadcasts over the network
// so every player sees the water play/stop. Needs a PhotonView on the object.
public class FireHose : MonoBehaviourPun
{
    [Header("References")]
    public ParticleSystem waterSystem;

    bool lastSent;

    void Start()
    {
        if (waterSystem != null)
        {
            var main = waterSystem.main;
            main.playOnAwake = false;
            waterSystem.Stop();
        }
    }

    public void SetSpraying(bool on)
    {
        if (on == lastSent) return; // only send when it actually changes
        lastSent = on;

        if (PhotonNetwork.IsConnected)
            photonView.RPC(nameof(SetSprayingRPC), RpcTarget.All, on);
        else
            SetSprayingRPC(on);
    }

    [PunRPC]
    void SetSprayingRPC(bool on)
    {
        if (waterSystem == null) return;
        if (on) waterSystem.Play();
        else waterSystem.Stop();
    }
}
