using UnityEngine;

public class SFXrandomizer : MonoBehaviour
{
    public AudioSource audioSource;

    private void Awake()
    {
        audioSource.pitch = Random.Range(0.85f, 1.15f);
    }
}
