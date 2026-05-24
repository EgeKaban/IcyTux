using System.Collections;
using UnityEngine;

public class CameraShake : MonoBehaviour
{
    public static CameraShake Instance;

    private void Awake()
    {
        // Singleton kurulumu: Bu scripte her yerden CameraShake.Instance yazarak ulaşabilirsin.
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    /// <summary>
    /// Kamerayı belirli bir süre ve şiddetle sarsar.
    /// </summary>
    /// <param name="duration">Sarsıntının ne kadar süreceği (saniye)</param>
    /// <param name="magnitude">Sarsıntının şiddeti/büyüklüğü</param>
    public void Shake(float duration, float magnitude)
    {
        StartCoroutine(ShakeCoroutine(duration, magnitude));
    }

    private IEnumerator ShakeCoroutine(float duration, float magnitude)
    {
        // Kameranın sarsıntıya başlamadan önceki orijinal pozisyonunu kaydet
        Vector3 originalPosition = transform.localPosition;
        float elapsed = 0f;

        while (elapsed < duration)
        {
            // X ve Y eksenlerinde rastgele bir sarsıntı değeri üret
            float x = Random.Range(-1f, 1f) * magnitude;
            float y = Random.Range(-1f, 1f) * magnitude;

            // Kameranın pozisyonuna bu rastgele değeri ekle
            transform.localPosition = new Vector3(originalPosition.x + x, originalPosition.y + y, originalPosition.z);

            // Gerçek zamanlı delta time kullanıyoruz ki oyun yavaşlasa bile (zaman bükülmesi) sarsıntı etkilenmesin
            elapsed += Time.unscaledDeltaTime;

            // Bir sonraki frame'e kadar bekle
            yield return null;
        }

        // Sarsıntı bittiğinde kamerayı kusursuz bir şekilde eski yerine koy
        transform.localPosition = originalPosition;
    }
}