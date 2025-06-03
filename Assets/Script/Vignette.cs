using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class Vignette : MonoBehaviour
{
    public bool triggerVignette = false; 
    public float maxIntensityValue = 0.5f;
    public float valueSpeed = 0.02f;

    private Volume volume;
    private UnityEngine.Rendering.Universal.Vignette vignette;

    void Start()
    {
        volume = GetComponent<Volume>();

        if (volume == null)
        {
            return;
        }

        if (!volume.profile.TryGet(out vignette))
        {
            Debug.LogError("Vignette ‚ª Volume Profile ‚ÉŠÜ‚Ü‚ê‚Ä‚¢‚Ü‚¹‚ñI");
        }
    }

    void FixedUpdate()
    {
        if (triggerVignette && vignette != null)
        {
            vignette.intensity.overrideState = true;
            vignette.intensity.value += valueSpeed;
            if (vignette.intensity.value >= maxIntensityValue)
            {
                vignette.intensity.value = maxIntensityValue;
            }
        }

        if (!triggerVignette && vignette != null)
        {
            vignette.intensity.value -= valueSpeed;
            if (vignette.intensity.value <= 0)
            {
                vignette.intensity.overrideState = false;
                vignette.intensity.value = 0;
            }
        }

    }
}
