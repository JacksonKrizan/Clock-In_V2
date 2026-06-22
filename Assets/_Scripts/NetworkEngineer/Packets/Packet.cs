using System.Collections;
using UnityEngine;
using Photon.Pun;

public class Packet : MonoBehaviourPun, IPunObservable
{
    [Header("Settings")]
    public string packetTag = "Packages";
    public Color deliveryColor = Color.cyan;
    
    private Renderer rend;
    private Rigidbody rb;
    private bool isDelivered = false;
    PacketSpawner packetSpawner;

    void Awake()
    {
        rend = GetComponent<Renderer>();
        rb = GetComponent<Rigidbody>();
        packetSpawner = FindObjectOfType<PacketSpawner>();
    }
    public void Update()
    {
        FallOutOfBoundsCheck();
    }

    void Start()
    {
        if (rend != null)
        {
            rend.material.color = deliveryColor;
            // Add a glowing effect
            rend.material.EnableKeyword("_EMISSION");
            rend.material.SetColor("_EmissionColor", deliveryColor * 0.5f);
        }
    }

    [PunRPC]
    public void Deliver()
    {
        if (isDelivered) return;
        isDelivered = true;
        
        Debug.Log("Packet Delivered!");
        
        // Change color to green on delivery
        if (rend != null)
        {
            rend.material.color = Color.green;
            rend.material.SetColor("_EmissionColor", Color.green * 0.8f);
        }

        // Change tag so it can't be picked up again
        gameObject.tag = "Untagged"; 
        
        // Optional: Master client could handle scoring here
    }

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting)
        {
            stream.SendNext(isDelivered);
        }
        else
        {
            bool wasDelivered = isDelivered;
            isDelivered = (bool)stream.ReceiveNext();
            
            if (isDelivered && !wasDelivered)
            {
                // Sync state if it changed while we weren't looking
                if (rend != null)
                {
                    rend.material.color = Color.green;
                    rend.material.SetColor("_EmissionColor", Color.green * 0.8f);
                }
                gameObject.tag = "Untagged";
            }
        }
    }

    void FallOutOfBoundsCheck()
    {
        if (transform.position.y < -10f)
        {
            if (packetSpawner != null) packetSpawner.currentPacketCount--;
            Destroy(gameObject);
        }
    }

    // called by IPdraggable when the player picks this packet up
    public void OnPickedUp()
    {
        if (packetSpawner != null) packetSpawner.ShowHud();
        if (MiniGameHUD.Instance != null) MiniGameHUD.Instance.ShowResponse("Carry the packet to the goal");
    }

    // called by PacketGoal when the packet reaches the destination.
    // returns true only the first time (so the goal scores it once).
    public bool OnDelivered()
    {
        if (isDelivered) return false;
        isDelivered = true;

        if (rend != null)
        {
            rend.material.color = Color.green;
            rend.material.SetColor("_EmissionColor", Color.green * 0.8f);
        }
        gameObject.tag = "Untagged";

        if (MiniGameHUD.Instance != null) MiniGameHUD.Instance.ShowResponse("Delivered!");
        StartCoroutine(DespawnAfter(3f));
        return true;
    }

    IEnumerator DespawnAfter(float seconds)
    {
        yield return new WaitForSeconds(seconds);
        // lower the count so the spawner makes a new one
        if (packetSpawner != null) packetSpawner.currentPacketCount--;
        Destroy(gameObject);
    }
}
