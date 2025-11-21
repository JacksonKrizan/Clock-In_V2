using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class Clickable : MonoBehaviour
{
    [SerializeField] List<GameObject> objectsToToggle;
    [SerializeField] List<string> tagsToToggle = new List<string>();


    void Update()
    { //Debug.Log("1 Clickable");
        if (Input.GetMouseButtonDown(0) && !EventSystem.current.IsPointerOverGameObject())
        {Debug.Log("2 Clickable");
        //Debug.Log(hit.collider.tag);
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out RaycastHit hit))
            {Debug.Log("3 Clickable");
            Debug.Log(hit.collider.tag);
                //Transform hitT = hit.collider.transform;
                for (int i = 0; i < tagsToToggle.Count; i++)
                {Debug.Log("4 Clickable");
                    Debug.Log(hit.collider.tag);
                    //var target = mapObjects[i];
                    
                    //if (hitT == target.transform || hitT.IsChildOf(target.transform))
                    if (hit.collider.CompareTag(tagsToToggle[i]))
                    {
                        Debug.Log("5 Clickable");
                        //SelectMap(i);
                        Debug.Log("Clicked on " + tagsToToggle[i]);
                        objectsToToggle[i].SetActive(!objectsToToggle[i].activeSelf);
                        return;
                    }
                }
            }
        }
    }


}
