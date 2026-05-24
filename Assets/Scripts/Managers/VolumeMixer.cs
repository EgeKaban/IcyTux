using UnityEngine;
using UnityEngine.Audio;

/// <summary>
/// AudioMixer üzerindeki ses seviyelerini ayarlamak için kullanılır.
/// UI Slider'ları (0.0001 ile 1 arası değerli) ile kullanılabilir.
/// </summary>
public class VolumeMixer : MonoBehaviour
{
    [Header("Audio Mixer")]
    public AudioMixer audioMixer;

    [Header("Exposed Parameters")]
    [Tooltip("Audio Mixer'da expose edilmiş Master volume parametre adı")]
    public string masterVolumeParam = "MasterVolume";
    //[Tooltip("Audio Mixer'da expose edilmiş Music volume parametre adı")]
    //public string musicVolumeParam = "MusicVolume";
    //[Tooltip("Audio Mixer'da expose edilmiş SFX volume parametre adı")]
    //public string sfxVolumeParam = "SFXVolume";

    /// <summary>
    /// Slider'dan gelen 0.0001 - 1.0 arası değeri alır ve Decibel'e çevirip Mixer'a uygular.
    /// Slider'ın OnValueChanged event'ine bağlanabilir.
    /// </summary>
    public void SetMasterVolume(float sliderValue)
    {
        if (audioMixer == null) return;
        // db = Log10(value) * 20
        float dbValue = Mathf.Log10(Mathf.Clamp(sliderValue, 0.0001f, 1f)) * 20f;
        audioMixer.SetFloat(masterVolumeParam, dbValue);
    }

    //public void SetMusicVolume(float sliderValue)
    //{
    //    if (audioMixer == null) return;
    //    float dbValue = Mathf.Log10(Mathf.Clamp(sliderValue, 0.0001f, 1f)) * 20f;
    //    audioMixer.SetFloat(musicVolumeParam, dbValue);
    //}

    //public void SetSFXVolume(float sliderValue)
    //{
    //    if (audioMixer == null) return;
    //    float dbValue = Mathf.Log10(Mathf.Clamp(sliderValue, 0.0001f, 1f)) * 20f;
    //    audioMixer.SetFloat(sfxVolumeParam, dbValue);
    //}

    /// <summary>
    /// Doğrudan db değeri atamak için.
    /// </summary>
    public void SetVolumeRaw(string paramName, float dbValue)
    {
        if (audioMixer == null) return;
        audioMixer.SetFloat(paramName, dbValue);
    }
}
