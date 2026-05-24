using UnityEngine;
using System.Collections; // Coroutine için eklendi

public class RoomCamera : MonoBehaviour
{
    public static RoomCamera Instance; // Her yerden kolayca ulaşmak için Singleton eklendi

    [Header("Targets")]
    [Tooltip("Ana karakterinizi buraya sürükleyin.")]
    public Transform player;

    [Header("Camera Settings")]
    public float smoothSpeed = 8f;
    [Tooltip("Kamera size geçiş hızı (orthographic size değişirken).")]
    public float sizeSmoothSpeed = 5f;

    private Bounds currentRoomBounds;
    private Camera cam;

    private float camHalfHeight;
    private float camHalfWidth;

    // Hedef orthographic size (odalar arası geçişte kullanılır)
    private float defaultSize;
    private float targetSize;

    // --- EKLENEN DEĞİŞKENLER ---
    private float currentRoomSize; // Odanın dönmesi gereken asıl boyutunu hafızada tutar
    private Coroutine zoomCoroutine;

    private void Awake()
    {
        if (Instance == null) Instance = this;
    }

    void Start()
    {
        cam = Camera.main;
        defaultSize = cam.orthographicSize;

        targetSize = defaultSize;
        currentRoomSize = defaultSize; // Başlangıçta odanın asıl boyutu default

        UpdateCamDimensions();
    }

    /// <summary>
    /// Yeni odaya geçiş (kamera size değiştirmeden).
    /// </summary>
    public void SetNewRoom(Bounds roomBounds)
    {
        currentRoomBounds = roomBounds;
        targetSize = defaultSize;
        currentRoomSize = defaultSize; // Hafızayı güncelle
    }

    /// <summary>
    /// Yeni odaya geçiş + kamera size override.
    /// </summary>
    public void SetNewRoom(Bounds roomBounds, float newCameraSize)
    {
        currentRoomBounds = roomBounds;
        targetSize = newCameraSize;
        currentRoomSize = newCameraSize; // Hafızayı güncelle
    }

    // --- ZOOM EFEKTİ METOTLARI ---

    /// <summary>
    /// Kamerayı geçici süreliğine büyütür veya küçültür, sonra eski haline döndürür.
    /// </summary>
    /// <param name="zoomSize">Kameranın ulaşacağı yeni orthographic size (küçültmek için düşük değer verin)</param>
    /// <param name="duration">Zoom'un ekranda kalacağı süre</param>
    /// <param name="customSpeed">Kameranın bu boyuta ulaşma hızı (Anında girmesi için yüksek bir değer verebilirsin. -1 varsayılan hızı kullanır)</param>
    public void TriggerZoomEffect(float zoomSize, float duration, float customSpeed = -1f)
    {
        if (zoomCoroutine != null) StopCoroutine(zoomCoroutine);
        zoomCoroutine = StartCoroutine(ZoomRoutine(Camera.main.orthographicSize - zoomSize, duration, customSpeed));
    }

    private IEnumerator ZoomRoutine(float zoomSize, float duration, float customSpeed)
    {
        float originalSpeed = sizeSmoothSpeed;

        // Özel bir hız verildiyse (örneğin dash atarken kameranın bir anda daralması için) onu ayarla
        if (customSpeed > 0) sizeSmoothSpeed = customSpeed;

        // Hedefi zoom boyutu yap, LateUpdate oraya otomatik kayacak
        targetSize = zoomSize;

        // Zamanı yavaşlatıyorsan (Time.timeScale) bu sürenin etkilenmemesi için Realtime kullanıyoruz
        yield return new WaitForSecondsRealtime(duration);

        // Süre dolunca hedefi odanın asıl boyutuna geri çek ve hızı sıfırla
        targetSize = currentRoomSize;
        sizeSmoothSpeed = originalSpeed;
    }
    // ----------------------------

    void LateUpdate()
    {
        if (player == null || currentRoomBounds.size == Vector3.zero) return;

        // Orthographic size'ı smooth olarak hedefe yaklaştır
        if (!Mathf.Approximately(cam.orthographicSize, targetSize))
        {
            cam.orthographicSize = Mathf.Lerp(cam.orthographicSize, targetSize, sizeSmoothSpeed * Time.deltaTime);

            // Çok yakınsa snap yap (titreme önleme)
            if (Mathf.Abs(cam.orthographicSize - targetSize) < 0.01f)
                cam.orthographicSize = targetSize;

            UpdateCamDimensions();
        }

        // Oda sınırlarını kullanarak pürüzsüzce hareket et
        MoveTo(player.position, false);
    }

    private void UpdateCamDimensions()
    {
        camHalfHeight = cam.orthographicSize;
        camHalfWidth = camHalfHeight * cam.aspect;
    }

    private void MoveTo(Vector3 targetPos, bool ignoreBounds)
    {
        Vector3 finalTargetPosition = targetPos;
        finalTargetPosition.z = transform.position.z;

        if (!ignoreBounds)
        {
            float minX = currentRoomBounds.min.x + camHalfWidth;
            float maxX = currentRoomBounds.max.x - camHalfWidth;
            float minY = currentRoomBounds.min.y + camHalfHeight;
            float maxY = currentRoomBounds.max.y - camHalfHeight;

            if (currentRoomBounds.size.x < camHalfWidth * 2)
            {
                finalTargetPosition.x = currentRoomBounds.center.x;
            }
            else
            {
                finalTargetPosition.x = Mathf.Clamp(finalTargetPosition.x, minX, maxX);
            }

            if (currentRoomBounds.size.y < camHalfHeight * 2)
            {
                finalTargetPosition.y = currentRoomBounds.center.y;
            }
            else
            {
                finalTargetPosition.y = Mathf.Clamp(finalTargetPosition.y, minY, maxY);
            }
        }

        transform.position = Vector3.Lerp(transform.position, finalTargetPosition, smoothSpeed * Time.deltaTime);
    }
}