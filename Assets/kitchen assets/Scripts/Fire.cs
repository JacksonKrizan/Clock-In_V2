using UnityEngine;

[ExecuteAlways]
public class Fire : MonoBehaviour
{
    [Header("Adjust this slider (0 = Off, 1 = Full)")]
    // Changed to public so Extinguisher can access it
    [Range(0f, 1f)] public float currentIntensity = 1.0f;

    [Header("Settings")]
    [SerializeField] private ParticleSystem firePS = null;
    [Tooltip("The particle count when the slider is at 1.0")]
    [SerializeField] private float maxEmissionRate = 10.0f; 

    private void Update()
    {
        if (firePS == null) return;

        var emission = firePS.emission;
        float newRate = currentIntensity * maxEmissionRate;
        emission.rateOverTime = newRate;

        // Toggle emission based on intensity
        emission.enabled = newRate > 0;
    }
}