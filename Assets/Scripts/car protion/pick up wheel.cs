using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PickUpWheel1 : MonoBehaviour
{
    
    public GameObject WheelOnPlayer1;

    void Start()
    {
        WheelOnPlayer1.SetActive(false);
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            if (Input.GetKey(KeyCode.E))
            {
                gameObject.SetActive(false);
                WheelOnPlayer1.SetActive(true);
            }
        }
    }
}