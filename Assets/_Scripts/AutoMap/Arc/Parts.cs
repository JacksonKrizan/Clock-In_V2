using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class Parts : MonoBehaviour
{

    public GameObject WheelOnPlayer1;
    public List<GameObject> carParts = new List<GameObject>();

    void Start()
    {
        WheelOnPlayer1.SetActive(false);
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            if (Input.GetKey(KeyCode.E))
            {
                gameObject.SetActive(false);
                WheelOnPlayer1.SetActive(true);
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
                Debug.Log("workedcarpart2");
                Transform hitT = hit.collider.transform;
                for (int i = 0; i < carParts.Count; i++)
                {
                    var target = carParts[i];
                    if (target == null) continue;
                    if (hitT == target.transform || hitT.IsChildOf(target.transform))
                    {
                        Debug.Log("workedcarpart");
                        return;
                    }
                }
            }
        }
    }
}

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