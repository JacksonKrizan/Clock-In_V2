using UnityEngine;

// Put this on a fire alongside the Fire script. When water particles hit it, it
// tells the Fire to lose intensity. The Fire handles the actual value + syncing,
// so a hit from any player counts for everyone.
public class FireExtinguisher : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Fire fireScript;

    [Header("Extinguish Settings")]
    [Tooltip("How much intensity to remove per particle hit")]
    [SerializeField] private float extinguishStrength = 0.01f;

    private void OnParticleCollision(GameObject other)
    {
        if (fireScript != null)
            fireScript.Extinguish(extinguishStrength);
    }
}
