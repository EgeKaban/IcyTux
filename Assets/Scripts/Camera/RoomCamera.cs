using UnityEngine;
using System.Collections;

public class RoomCamera : MonoBehaviour
{
    public static RoomCamera Instance;

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

    private float defaultSize;
    private float targetSize;

    private float currentRoomSize;
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
        currentRoomSize = defaultSize;

        UpdateCamDimensions();
    }

    public void SetNewRoom(Bounds roomBounds)
    {
        currentRoomBounds = roomBounds;
        targetSize = defaultSize;
        currentRoomSize = defaultSize;
    }

    public void SetNewRoom(Bounds roomBounds, float newCameraSize)
    {
        currentRoomBounds = roomBounds;
        targetSize = newCameraSize;
        currentRoomSize = newCameraSize;
    }

    public void TriggerZoomEffect(float zoomSize, float duration, float customSpeed = -1f)
    {
        if (zoomCoroutine != null) StopCoroutine(zoomCoroutine);
        zoomCoroutine = StartCoroutine(ZoomRoutine(Camera.main.orthographicSize - zoomSize, duration, customSpeed));
    }

    private IEnumerator ZoomRoutine(float zoomSize, float duration, float customSpeed)
    {
        float originalSpeed = sizeSmoothSpeed;

        if (customSpeed > 0) sizeSmoothSpeed = customSpeed;

        targetSize = zoomSize;

        yield return new WaitForSecondsRealtime(duration);

        targetSize = currentRoomSize;
        sizeSmoothSpeed = originalSpeed;
    }

    void LateUpdate()
    {
        if (player == null || currentRoomBounds.size == Vector3.zero) return;

        if (!Mathf.Approximately(cam.orthographicSize, targetSize))
        {
            cam.orthographicSize = Mathf.Lerp(cam.orthographicSize, targetSize, sizeSmoothSpeed * Time.deltaTime);

            if (Mathf.Abs(cam.orthographicSize - targetSize) < 0.01f)
                cam.orthographicSize = targetSize;

            UpdateCamDimensions();
        }

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