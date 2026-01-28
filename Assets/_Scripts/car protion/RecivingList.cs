using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RecivingList : MonoBehaviour
{
    public TestVarList varList;
    private void Update()
    {
        for(int i = 0;i < varList.partList.Count; i++)
        {
            Debug.Log(varList.partList[i]);
            Debug.Log("works");
        }
        
    }
}
