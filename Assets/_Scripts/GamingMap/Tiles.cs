using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tiles : MonoBehaviour
{
    public List<GameObject> targets = new List<GameObject>();
    public Color glowColor = Color.cyan;

    public float speed = UnityEngine.Random.Range(1.5f, 5.0f);



        void Start() 
        {
            
            InvokeRepeating("TilesFaded", speed);
        }

        void TilesFaded() 
        {
            speed = UnityEngine.Random.Range(1.5f, 5.0f);
            Debug.Log("Executed every 5 seconds!");
        }


    void Update()
    {






        float intensity = Mathf.PingPong(Time.time * speed, 1f);
        foreach (var go in targets)
        {
            if (go == null) continue;
            foreach (var r in go.GetComponentsInChildren<Renderer>())
            {
                var mat = r.material;
                if (mat && mat.HasProperty("_EmissionColor"))
                    mat.SetColor("_EmissionColor", glowColor * intensity);
            }
        }
    }
}
