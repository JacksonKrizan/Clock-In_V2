using UnityEngine;
using System.Collections;

public class ItemPlace : MonoBehaviour
{
    public Transform targetEmpty; // The destination
    public float interactRange = 3.0f; // How close you must be
    public float delayTime;
    public bool place = false;
    public float moveSpeed = 5f;
    public float rotateSpeed = 5f;

    void Update()
    {
        //float distance = 3.0f;//= Vector3.Distance(transform.position, targetEmpty.position);
        float distance = Vector3.Distance(transform.position, targetEmpty.position);
        float rotate = Vector3.Distance(transform.rotation.eulerAngles, targetEmpty.rotation.eulerAngles);

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
        if (place == true)
        {
            transform.position = targetEmpty.position;
            transform.rotation = targetEmpty.rotation;
            //transform.position = Vector3.Lerp(transform.position,targetEmpty.position, moveSpeed * Time.deltaTime);
            //transform.rotation = Quaternion.Lerp(transform.rotation,targetEmpty.rotation,rotateSpeed * Time.deltaTime);
            place = false;
        }
    }
    IEnumerator PlaceObject(float delayTime)
    {
        yield return new WaitForSeconds(.01f);
        place = true;
    }
}

