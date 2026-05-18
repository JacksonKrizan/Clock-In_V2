/*using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class Clickable : MonoBehaviour
{
    [SerializeField] List<GameObject> objectsToToggle;
    [SerializeField] List<string> tagsToToggle = new List<string>();


    /*void Update()
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
                    objectsToToggle[i].SetActive(!objectsToToggle[i].activeSelf);
                    //var target = mapObjects[i];
                    
                    //if (hitT == target.transform || hitT.IsChildOf(target.transform))
                    if (hit.collider.CompareTag(tagsToToggle[i]))
                    {
                        Debug.Log("5 Clickable");
                        //SelectMap(i);
                        Debug.Log("Clicked on " + tagsToToggle[i]);
                        objectsToToggle[0].SetActive(!objectsToToggle[0].activeSelf);
                        return;
                    }
                }
            }
        }
    }
    void Update()
    {
        if (Input.GetMouseButtonDown(0) && !EventSystem.current.IsPointerOverGameObject())
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out RaycastHit hit))
            {
                Debug.Log("Hit: " + hit.collider.gameObject.name + " with tag: " + hit.collider.tag);
                
                for (int i = 0; i < tagsToToggle.Count; i++)
                {
                    if (hit.collider.CompareTag(tagsToToggle[i]) && objectsToToggle[i] != null)
                    {
                        objectsToToggle[i].SetActive(!objectsToToggle[i].activeSelf);
                        Debug.Log("Toggled " + objectsToToggle[i].name);
                        return;
                    }
                }
            }
        }
    }
}

using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class Clickable : MonoBehaviour
{
    [System.Serializable]
    public class TagTogglePair
    {
        public string tag;
        public GameObject target;
    }

    public List<TagTogglePair> togglePairs = new List<TagTogglePair>();

    void Update()
    {
        if (Input.GetMouseButtonDown(0) &&
            !EventSystem.current.IsPointerOverGameObject())
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

            if (Physics.Raycast(ray, out RaycastHit hit))
            {
                string hitTag = hit.collider.tag;
                Debug.Log("Hit: " + hit.collider.name + " Tag: " + hitTag);

                foreach (var pair in togglePairs)
                {
                    if (hitTag == pair.tag && pair.target != null)
                    {
                        pair.target.SetActive(!pair.target.activeSelf);
                        Debug.Log("Toggled: " + pair.target.name);
                        return;
                    }
                }
            }
        }
    }
}*/
using UnityEngine;
using UnityEngine.EventSystems;

public class Clickable : MonoBehaviour
{
    [SerializeField] GameObject objectToToggle;

    void Update()
    {
        if (Input.GetMouseButtonDown(0) &&
            !EventSystem.current.IsPointerOverGameObject())
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out RaycastHit hit))
            {
                Debug.Log(hit.collider.tag);
                // Check for the tag "server"
                if (hit.collider.CompareTag("Untagged"))
                {
                    objectToToggle.SetActive(!objectToToggle.activeSelf);
                    Debug.Log("Toggled: " + objectToToggle.name);
                }
            }
        }
    }
}

