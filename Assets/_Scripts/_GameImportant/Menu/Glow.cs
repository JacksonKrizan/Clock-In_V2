using System.Collections.Generic;
using UnityEngine;

public class Glow : MonoBehaviour
{
    public List<GameObject> targets = new List<GameObject>();
    public Color glowColor = Color.cyan;
    public float speed = 2f;

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
