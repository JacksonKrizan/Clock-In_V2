using UnityEngine;
using Photon.Pun;
using Photon.Realtime;

// Makes the hose a one-person tool. Whoever grabs it takes ownership of its
// PhotonView (so their PhotonTransformView drives its movement for everyone), and
// nobody else can grab it until they drop it. Put this on the hose alongside a
// PhotonView (Ownership Transfer = Takeover), a PhotonTransformView, and a Rigidbody.
[RequireComponent(typeof(PhotonView))]
public class HoseControl : MonoBehaviourPunCallbacks
{
    int heldByActor; // 0 = free, otherwise the actor number holding it
    Rigidbody rb;

    void Awake() { rb = GetComponent<Rigidbody>(); }

    void Update()
    {
        if (rb == null) return;
        // only the owner runs physics, and only while the hose is free. everyone
        // else (and the holder) just shows the network-driven position, so it
        // can't fall on other screens.
        bool kinematic = !photonView.IsMine || heldByActor != 0;
        if (rb.isKinematic != kinematic) rb.isKinematic = kinematic;

        FallOutOfBoundsCheck();
    }

    // if the hose drops out of the world, the owner removes it; the HoseSpawner
    // notices the count dropped and spawns a fresh one.
    void FallOutOfBoundsCheck()
    {
        if (transform.position.y < -10f)
            DestroyHose();
    }

    void DestroyHose()
    {
        if (PhotonNetwork.IsConnected)
        {
            if (photonView.IsMine) PhotonNetwork.Destroy(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    int MyActor => PhotonNetwork.IsConnected ? PhotonNetwork.LocalPlayer.ActorNumber : 1;

    public bool IsFree => heldByActor == 0;

    // try to take the hose; false if someone else already has it
    public bool TryGrab()
    {
        if (heldByActor != 0 && heldByActor != MyActor) return false;

        if (PhotonNetwork.IsConnected)
        {
            photonView.RequestOwnership();
            photonView.RPC(nameof(SetHolderRPC), RpcTarget.AllBuffered, MyActor);
        }
        else SetHolderRPC(MyActor);
        return true;
    }

    public void Release()
    {
        if (heldByActor != MyActor) return;

        if (PhotonNetwork.IsConnected)
            photonView.RPC(nameof(SetHolderRPC), RpcTarget.AllBuffered, 0);
        else SetHolderRPC(0);
    }

    [PunRPC]
    void SetHolderRPC(int actor)
    {
        heldByActor = actor;
        // kinematic state is handled per-client in Update based on ownership
    }

    // if the holder disconnects, the master frees the hose
    public override void OnPlayerLeftRoom(Player player)
    {
        if (PhotonNetwork.IsMasterClient && heldByActor == player.ActorNumber)
            photonView.RPC(nameof(SetHolderRPC), RpcTarget.AllBuffered, 0);
    }
}
