using UnityEngine;

public class WheelAttach : MonoBehaviour
{
    public GameObject WheelOnPlayer1;   // The wheel shown on the player
    public Transform AttachPoint;       // Where the wheel should attach on the car
    private bool wheelPickedUp = false;

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

    void AttachWheel()
    {
        WheelOnPlayer1.transform.SetParent(AttachPoint);
        WheelOnPlayer1.transform.localPosition = Vector3.zero;
        WheelOnPlayer1.transform.localRotation = Quaternion.identity;

        Collider col = WheelOnPlayer1.GetComponent<Collider>();
        if (col) col.enabled = false;

        Rigidbody rb = WheelOnPlayer1.GetComponent<Rigidbody>();
        if (rb) rb.isKinematic = true;

        Debug.Log("Wheel attached!");
    }
}
