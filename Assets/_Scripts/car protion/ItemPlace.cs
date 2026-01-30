using UnityEngine;
using System.Collections;

public class ItemPlace : MonoBehaviour
{
    public Transform targetEmpty; // The destination
    public float interactRange = 3.0f; // How close you must be
    public float delayTime;

    void Update()
    {
        float distance = Vector3.Distance(transform.position, targetEmpty.position);

        if (Input.GetKeyDown(KeyCode.F))
        {
            Debug.Log("F was pressed! Current distance is: " + distance);
        
            if (distance <= interactRange)
            {
                Debug.Log("In range! Teleporting...");
                StartCoroutine(PlaceObject(delayTime));
                
            }
            else 
            {
                Debug.Log("Too far away to teleport.");
            }
        }
    }
    IEnumerator PlaceObject(float delayTime)
    {
        yield return new WaitForSeconds(.01f);
        transform.position = targetEmpty.position;
    }
}

