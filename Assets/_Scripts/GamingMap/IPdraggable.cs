using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class IPdraggable : MonoBehaviour
{
  
    public GameObject player;
    public Transform holdPos;
    public float throwForce = 500f;
    public float pickUpRange = 5f;
    private float rotationSensitivity = 1f; 
    private GameObject heldObj;
    private Rigidbody heldObjRb;
    private bool canDrop = true; 
    private int LayerNumber;

    [SerializeField] List<string> pickUpTags = new List<string>();
    private TestVarList GetVarList;


    void Start()
    {
        LayerNumber = LayerMask.NameToLayer("holdLayer");
        if (LayerNumber == -1)
        {
            Debug.LogWarning("Layer 'holdLayer' not found in Tag and Layer settings. Please create it! Defaulting to layer 0.");
            LayerNumber = 0;
        }
    }
    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            if (heldObj == null)
            {
                RaycastHit hit;
                if (Physics.Raycast(transform.position, transform.TransformDirection(Vector3.forward), out hit, pickUpRange))
                {
                    for (int i = 0; i < pickUpTags.Count; i++)
                    {
                        if (hit.transform.gameObject.tag == pickUpTags[i])
                        {
                            PickUpObject(hit.transform.gameObject);
                            break; // Exit loop after picking up
                        }
                    }
                }
            }
            else
            {
                if (canDrop == true)
                {
                    StopClipping();
                    DropObject();
                }
            }
        }
        if (Input.GetMouseButtonDown(1) && heldObj != null)
        {
            ThrowObject();
        }

        if (heldObj != null)
        {
            MoveObject();
            RotateObject();
            // Removed second GetMouseButtonDown check here to prevent double-triggering
        }
    }




    void PickUpObject(GameObject pickUpObj)
    {
        if (pickUpObj.GetComponent<Rigidbody>())
        {
            heldObj = pickUpObj;
            heldObjRb = pickUpObj.GetComponent<Rigidbody>();
            heldObjRb.isKinematic = true;
            heldObjRb.transform.parent = holdPos.transform; 
            heldObj.layer = LayerNumber;
            Physics.IgnoreCollision(heldObj.GetComponent<Collider>(), player.GetComponent<Collider>(), true);
        }
    }
    void DropObject()
    {
        Physics.IgnoreCollision(heldObj.GetComponent<Collider>(), player.GetComponent<Collider>(), false);
        heldObj.layer = 0;
        heldObjRb.isKinematic = false;
        heldObj.transform.parent = null; 
        heldObj = null; 
    }
    void MoveObject()
    {
  
        heldObj.transform.position = holdPos.transform.position;
    }
    void RotateObject()
    {
        if (Input.GetKey(KeyCode.R))
        {
            canDrop = false; 


            float XaxisRotation = Input.GetAxis("Mouse X") * rotationSensitivity;
            float YaxisRotation = Input.GetAxis("Mouse Y") * rotationSensitivity;

            heldObj.transform.Rotate(Vector3.down, XaxisRotation);
            heldObj.transform.Rotate(Vector3.right, YaxisRotation);
        }
        else
        {

            canDrop = true;
        }
    }
    void ThrowObject()
    {
        
        Physics.IgnoreCollision(heldObj.GetComponent<Collider>(), player.GetComponent<Collider>(), false);
        heldObj.layer = 0;
        heldObjRb.isKinematic = false;
        heldObj.transform.parent = null;
        heldObjRb.AddForce(transform.forward * throwForce);
        heldObj = null;
    }
    void StopClipping()
    {
        var clipRange = Vector3.Distance(heldObj.transform.position, transform.position);

        RaycastHit[] hits;
        hits = Physics.RaycastAll(transform.position, transform.TransformDirection(Vector3.forward), clipRange);

        if (hits.Length > 1)
        {

            heldObj.transform.position = transform.position + new Vector3(0f, -0.5f, 0f); 
        }
    }
}
