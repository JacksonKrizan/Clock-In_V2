using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;


public class EthicalHacker : MonoBehaviour
{
    public GameObject laptop;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButtonDown(0) && !EventSystem.current.IsPointerOverGameObject())
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out RaycastHit hit))
            {
                var target = laptop;
                Transform hitT = hit.collider.transform;
                    if (hitT == target.transform || hitT.IsChildOf(target.transform))
                    {
                    //stuff happens
                    Debug.Log("testing");
                        return;
                    }
                
            }
        }
    }
}