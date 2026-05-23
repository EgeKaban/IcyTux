using UnityEngine;

public class RoomCamera : MonoBehaviour
{
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

    void Start()
    {
        transform.parent = null;
        cam = Camera.main;
        defaultSize = cam.orthographicSize;
        targetSize = defaultSize;
        UpdateCamDimensions();
    }

    /// <summary>
    /// Yeni odaya geçiş (kamera size değiştirmeden).
    /// </summary>
    public void SetNewRoom(Bounds roomBounds)
    {
        currentRoomBounds = roomBounds;
        targetSize = defaultSize;
    }

    /// <summary>
    /// Yeni odaya geçiş + kamera size override.
    /// </summary>
    public void SetNewRoom(Bounds roomBounds, float newCameraSize)
    {
        currentRoomBounds = roomBounds;
        targetSize = newCameraSize;
    }

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

    /// <summary>
    /// Kamera yarı boyutlarını günceller (size değiştiğinde çağrılmalı).
    /// </summary>
    private void UpdateCamDimensions()
    {
        camHalfHeight = cam.orthographicSize;
        camHalfWidth = camHalfHeight * cam.aspect;
    }

    // Kamera hareketini ve sınırlamayı yöneten yardımcı fonksiyon
    private void MoveTo(Vector3 targetPos, bool ignoreBounds)
    {
        Vector3 finalTargetPosition = targetPos;
        finalTargetPosition.z = transform.position.z;

        // Eğer tuşa basılı tutuyorsak, oda sınırlarını atla
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