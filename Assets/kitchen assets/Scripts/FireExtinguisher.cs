using UnityEngine;

public class FireExtinguisher : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Fire fireScript;

    [Header("Extinguish Settings")]
    [Tooltip("How much intensity to remove per particle hit")]
    [SerializeField] private float extinguishStrength = 0.01f;

    [Tooltip("How quickly the fire recovers (0 = stays out)")]
    [SerializeField] private float recoveryRate = 0.05f;

    private void Update()
    {
        if (fireScript == null) return;

        // Slowly grow the fire back over time if it's not fully out
        if (fireScript.currentIntensity > 0 && fireScript.currentIntensity < 1.0f)
        {
            fireScript.currentIntensity += recoveryRate * Time.deltaTime;
            fireScript.currentIntensity = Mathf.Clamp01(fireScript.currentIntensity);
        }
    }

    private void OnParticleCollision(GameObject other)
    {
        // This is triggered when a particle hits this object's collider
        if (fireScript != null)
        {
            fireScript.currentIntensity -= extinguishStrength;
            fireScript.currentIntensity = Mathf.Clamp01(fireScript.currentIntensity);
        }
    }
}