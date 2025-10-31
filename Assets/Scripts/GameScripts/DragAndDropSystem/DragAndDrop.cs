using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DragAndDrop : MonoBehaviour
{
    private Vector3 offset;
    private float mouseZCoord;

    private void OnMouseDown()
    {
        // Store the mouse z-coordinate based on the object's distance from camera
        mouseZCoord = Camera.main.WorldToScreenPoint(transform.position).z;
        
        // Store offset between object position and mouse position
        offset = transform.position - GetMouseWorldPos();
    }

    private void OnMouseDrag()
    {
        transform.position = GetMouseWorldPos() + offset;
    }

    private Vector3 GetMouseWorldPos()
    {
        // Get mouse position in screen space
        Vector3 mousePoint = Input.mousePosition;
        
        // Set z to the distance from camera to object
        mousePoint.z = mouseZCoord;
        
        // Convert screen point to world point
        return Camera.main.ScreenToWorldPoint(mousePoint);
    }
}
