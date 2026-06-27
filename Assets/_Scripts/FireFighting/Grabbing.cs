using System.Collections.Generic;
using UnityEngine;

// Super simple pickup: left-click to grab the object you're looking at, it sits
// at holdPos, left-click again to drop it. The held object is NOT parented - it
// follows holdPos in world space so a PhotonTransformView syncs its real position
// to other players.
public class Grabbing : MonoBehaviour
{
    public Transform holdPos;        // where the held object sits
    public float pickUpRange = 5f;
    public List<string> pickUpTags = new List<string>(); // only these tags can be grabbed

    [Header("HUD")]
    [TextArea][SerializeField] string hoseHowTo = "Hold right-click while the hose is equipped to put out the fires.";

    GameObject heldObj;
    public GameObject HeldObject => heldObj; // what the player is currently holding (null if nothing)

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            if (heldObj == null) TryPickUp();
            else Drop();
        }

        // keep the held object pinned to holdPos in world space (no parenting)
        if (heldObj != null && holdPos != null)
        {
            heldObj.transform.position = holdPos.position;
            heldObj.transform.rotation = holdPos.rotation;
        }
    }

    void TryPickUp()
    {
        if (Physics.Raycast(transform.position, transform.forward, out RaycastHit hit, pickUpRange))
        {
            if (!pickUpTags.Contains(hit.transform.tag)) return; // not a grabbable tag

            Rigidbody rb = hit.transform.GetComponent<Rigidbody>();
            if (rb == null) return;

            // if it's the networked hose, only grab it if no one else has it
            HoseControl hose = hit.transform.GetComponent<HoseControl>();
            if (hose != null && !hose.TryGrab()) return;

            heldObj = hit.transform.gameObject;
            rb.isKinematic = true;

            // grabbing the hose -> show the fire mini-game how-to
            if (heldObj.GetComponent<FireHose>() != null && MiniGameHUD.Instance != null)
                MiniGameHUD.Instance.SetHowTo(hoseHowTo);
        }
    }

    void Drop()
    {
        // stop the water and release the hose lock so someone else can use it
        FireHose fireHose = heldObj.GetComponent<FireHose>();
        if (fireHose != null) fireHose.SetSpraying(false);

        HoseControl hose = heldObj.GetComponent<HoseControl>();
        if (hose != null) hose.Release();

        Rigidbody rb = heldObj.GetComponent<Rigidbody>();
        if (rb != null) rb.isKinematic = false;

        heldObj = null;
    }
}
