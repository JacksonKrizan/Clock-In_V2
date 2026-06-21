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
            //packetSpawner.currentPacketCount--;
            Destroy(gameObject);
        }
    }
}
