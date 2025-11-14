using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PickUpWheel : MonoBehaviour
{
    
    public GameObject WheelOnPlayer;

    void Start()
    {
        WheelOnPlayer.SetActive(false);
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            if (Input.GetKey(KeyCode.E))
            {
                gameObject.SetActive(false);
                WheelOnPlayer.SetActive(true);
            }
        }
    }
}