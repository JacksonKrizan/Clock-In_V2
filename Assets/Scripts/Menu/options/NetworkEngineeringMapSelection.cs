using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class NetworkEngineeringMapSelection : MonoBehaviour
{
   
    //public Material boxMaterial;
    //public Material selectMaterial;
    public Material highlightMaterial;
    public Material selectionMaterial;

    private Material originalMaterialHighlight;
    private Material originalMaterialSelection;
    private Transform highlight;
    private Transform selection;
    private RaycastHit raycastHit;

    // Start is called before the first frame update
    void Start()
    {
        // No need to find Launcher - we can use the static Instance
    }

    // Update is called once per frame
    void Update()
    {
        if (highlight != null)
        {
            Debug.Log("1");
            highlight.GetComponent<MeshRenderer>().sharedMaterial = originalMaterialHighlight;
            highlight = null;
        }
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        if (!EventSystem.current.IsPointerOverGameObject() && Physics.Raycast(ray, out raycastHit)) //Make sure you have EventSystem in the hierarchy before using EventSystem
        {
            highlight = raycastHit.transform;
            if (highlight.CompareTag("Networking") && highlight != selection)
            {
                if (highlight.GetComponent<MeshRenderer>().material != highlightMaterial)
                {
                    originalMaterialHighlight = highlight.GetComponent<MeshRenderer>().material;
                    highlight.GetComponent<MeshRenderer>().material = highlightMaterial;
                    Debug.Log("2");
                }
            }
            else
            {
                highlight = null;
                //Debug.Log("3");
            }
          
        }

        // Selection
        if (Input.GetMouseButtonDown(0) && !EventSystem.current.IsPointerOverGameObject())
        {
            if (highlight)
            {
                if (selection != null)
                {
                    Debug.Log("4");
                    selection.GetComponent<MeshRenderer>().material = originalMaterialSelection;
                }
                selection = raycastHit.transform;
                if (selection.GetComponent<MeshRenderer>().material != selectionMaterial)
                {
                    Debug.Log("5");
                    Launcher.Instance.mapNumber = 2;
                    Debug.Log(Launcher.Instance.mapNumber + " selected");
                    //originalMaterialSelection = originalMaterialHighlight;
                    selection.GetComponent<MeshRenderer>().material = selectionMaterial;
                }
                highlight = null;
            }
            else
            {
                if (selection)
                {
                    Debug.Log("6");
                    //footprintsUI.SetActive(!footprintsUI.gameObject.activeSelf);
                    Launcher.Instance.mapNumber = 1;
                    Debug.Log(Launcher.Instance.mapNumber + " deselected");
                    selection.GetComponent<MeshRenderer>().material = originalMaterialSelection;
                    selection = null;
                }
            }
        }

    
    }

}
