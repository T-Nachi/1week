using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class Vignette : MonoBehaviour
{
    public bool triggerVignette = false; 
    public float intensityValue = 0.5f;  

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
            Debug.LogError("Vignette Ç™ Volume Profile Ç…ä‹Ç‹ÇÍÇƒÇ¢Ç‹ÇπÇÒÅI");
        }
    }

    void Update()
    {
        if (triggerVignette && vignette != null)
        {
            vignette.intensity.overrideState = true;
            vignette.intensity.value = intensityValue;

            
            triggerVignette = false;
        }
    }
}
