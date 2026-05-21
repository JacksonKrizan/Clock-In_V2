using UnityEngine;

public class FireHose : MonoBehaviour
{
    [Header("References")]
    public ParticleSystem waterSystem; 
    
    [Header("Controls")]
    public bool useToggleMode = false; 

    private bool _isSpraying = false;

    void Start()
    {
        if (waterSystem != null)
        {
            var main = waterSystem.main;
            main.playOnAwake = false;
            waterSystem.Stop();
        }
    }

    void Update()
    {
        // "Fire2" is the standard Unity name for Right Mouse Button
        if (useToggleMode)
        {
            if (Input.GetButtonDown("Fire2"))
            {
                _isSpraying = !_isSpraying;
                UpdateWaterState();
            }
        }
        else
        {
            if (Input.GetButtonDown("Fire2"))
            {
                _isSpraying = true;
                UpdateWaterState();
            }
            else if (Input.GetButtonUp("Fire2"))
            {
                _isSpraying = false;
                UpdateWaterState();
            }
        }
    }

    void UpdateWaterState()
    {
        if (waterSystem == null) return;

        if (_isSpraying)
            waterSystem.Play();
        else
            waterSystem.Stop();
    }
}