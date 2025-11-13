using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class MapOption3D : MonoBehaviour
{
    [Tooltip("List of clickable map GameObjects. Index 0 = map 1, index 1 = map 2, etc.")]
    public List<GameObject> mapObjects = new List<GameObject>();

    public Material highlightMaterial;
    public Material selectionMaterial;

    private Transform selection;
    private Material originalSelectionMaterial;

    void Update()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        
        // Check for mouse click on a map object
        if (Input.GetMouseButtonDown(0) && !EventSystem.current.IsPointerOverGameObject())
        {
            if (Physics.Raycast(ray, out RaycastHit hit))
            {
                // Check if clicked object is in our list
                for (int i = 0; i < mapObjects.Count; i++)
                {
                    if (hit.collider.gameObject == mapObjects[i])
                    {
                        SelectMap(i);
                        return;
                    }
                }
            }
        }

        // Highlight on hover
        if (Physics.Raycast(ray, out RaycastHit hoverHit))
        {
            for (int i = 0; i < mapObjects.Count; i++)
            {
                if (hoverHit.collider.gameObject == mapObjects[i] && hoverHit.transform != selection)
                {
                    var renderer = hoverHit.collider.GetComponent<MeshRenderer>();
                    if (renderer && highlightMaterial)
                    {
                        renderer.material = highlightMaterial;
                    }
                }
            }
        }
    }

    void SelectMap(int mapIndex)
    {
        // Deselect previous
        if (selection != null)
        {
            var prevRenderer = selection.GetComponent<MeshRenderer>();
            if (prevRenderer && originalSelectionMaterial)
            {
                prevRenderer.material = originalSelectionMaterial;
            }
        }

        // Select new
        selection = mapObjects[mapIndex].transform;
        var renderer = selection.GetComponent<MeshRenderer>();
        if (renderer)
        {
            originalSelectionMaterial = renderer.material;
            if (selectionMaterial)
            {
                renderer.material = selectionMaterial;
            }
        }

        // Set map number (index + 1, so index 0 = map 1)
        Launcher.Instance.mapNumber = mapIndex + 1;
        Debug.Log($"Map {mapIndex + 1} selected");
    }
}
