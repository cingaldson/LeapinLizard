using UnityEngine;
using UnityEngine.UI;

public class VolumeControl : MonoBehaviour
{
    public AudioSource audioSource;

    public void OnSliderDrag()
    {
        // Update the volume based on the slider's value
        float newVolume = GetComponent<Slider>().value;
        audioSource.volume = newVolume;
    }
}
