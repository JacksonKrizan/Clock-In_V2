using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class MapOption3D : MonoBehaviour
{
    public List<GameObject> mapObjects = new List<GameObject>();
    public List<Material> mapObjectsOriginalMaterial = new List<Material>();
    public Material selectionMaterial;

    private int selectedIndex = -1;

    void Update()
    {
        if (Input.GetMouseButtonDown(0) && !EventSystem.current.IsPointerOverGameObject())
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out RaycastHit hit))
            {
                Transform hitT = hit.collider.transform;
                for (int i = 0; i < mapObjects.Count; i++)
                {
                    var target = mapObjects[i];
                    if (target == null) continue;
                    if (hitT == target.transform || hitT.IsChildOf(target.transform))
                    {
                        SelectMap(i);
                        return;
                    }
                }
            }
        }
    }

    void SelectMap(int index)
    {
        if (selectedIndex >= 0 && selectedIndex < mapObjects.Count)
        {
            var prevRenderer = mapObjects[selectedIndex].GetComponent<MeshRenderer>();
            if (prevRenderer)
            {
                if (selectedIndex < mapObjectsOriginalMaterial.Count && mapObjectsOriginalMaterial[selectedIndex] != null)
                {
                    prevRenderer.material = mapObjectsOriginalMaterial[selectedIndex];
                }
            }
        }

        selectedIndex = index;
        var renderer = mapObjects[index].GetComponent<MeshRenderer>();
        if (renderer && selectionMaterial)
        {
            while (mapObjectsOriginalMaterial.Count <= index)
                mapObjectsOriginalMaterial.Add(null);

            mapObjectsOriginalMaterial[index] = renderer.material;

            renderer.material = selectionMaterial;
        }

        Launcher.Instance.mapNumber = index + 1;
        Debug.Log($"Map {index + 1} selected");
    }
}
