using System.Collections.Generic;
using UnityEngine;

public class WheelAttach : MonoBehaviour
{
    public GameObject WheelOnPlayer1;   // The wheel shown on the player
    public Vector3 AttachPoint;       // Where the wheel should attach on the car
    //[SerializeField] GameObject carTire;
    private bool wheelPickedUp = false;
    public List<GameObject> wheels;

    void Start()
    {
        WheelOnPlayer1.SetActive(false);
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            if (!wheelPickedUp && Input.GetKey(KeyCode.E))
            {
                gameObject.SetActive(false);
                WheelOnPlayer1.SetActive(true);
                wheelPickedUp = true;
            }

            if (wheelPickedUp && Input.GetKeyDown(KeyCode.F))
            {
                AttachWheel();
            }
        }
    }
    void OnCollisionEnter(Collision collision)
    {
        if(collision.gameObject.CompareTag("Tirespot"))
        {
            AttachWheel();
            Debug.Log("tire");
        }
    }

    void AttachWheel()
    {
        //WheelOnPlayer1.transform.SetParent(AttachPoint);
        WheelOnPlayer1.transform.localPosition = Vector3.zero;
        WheelOnPlayer1.transform.localRotation = Quaternion.identity;

        Collider col = WheelOnPlayer1.GetComponent<Collider>();
        if (col) col.enabled = false;

        Rigidbody rb = WheelOnPlayer1.GetComponent<Rigidbody>();
        if (rb) rb.isKinematic = true;

        Debug.Log("Wheel attached!");
    }

    void AttachWheel2()
    {
        wheels[0].transform.position = new Vector3(0, 0, 0); 
    }
}
