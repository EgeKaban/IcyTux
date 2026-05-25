using UnityEngine;
using UnityEngine.Audio;

public class VolumeMixer : MonoBehaviour
{
    [Header("Audio Mixer")]
    public AudioMixer audioMixer;

    [Header("Exposed Parameters")]
    [Tooltip("Audio Mixer'da expose edilmiş Master volume parametre adı")]
    public string masterVolumeParam = "MasterVolume";

    public void SetMasterVolume(float sliderValue)
    {
        if (audioMixer == null) return;
        float dbValue = Mathf.Log10(Mathf.Clamp(sliderValue, 0.0001f, 1f)) * 20f;
        audioMixer.SetFloat(masterVolumeParam, dbValue);
    }

    public void SetVolumeRaw(string paramName, float dbValue)
    {
        if (audioMixer == null) return;
        audioMixer.SetFloat(paramName, dbValue);
    }
}
