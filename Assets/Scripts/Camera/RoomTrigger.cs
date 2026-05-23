using UnityEngine;

[RequireComponent(typeof(BoxCollider2D))]
public class RoomTrigger : MonoBehaviour
{
    private RoomCamera camScript;
    private BoxCollider2D roomCollider;

    [Header("Camera Size Override")]
    [Tooltip("Aktifleştirilirse bu odaya girildiğinde kameranın orthographic size'ı değişir.")]
    public bool overrideCameraSize = false;
    [Tooltip("Bu odadaki kamera orthographic size değeri.")]
    public float targetCameraSize = 5f;

    void Start()
    {
        camScript = Camera.main.GetComponent<RoomCamera>();
        roomCollider = GetComponent<BoxCollider2D>();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        // Odaya Player VEYA Shadow girdiğinde, kameranın kilit odasını güncelle
        if (collision.CompareTag("Player"))
        {
            if (overrideCameraSize)
                camScript.SetNewRoom(roomCollider.bounds, targetCameraSize);
            else
                camScript.SetNewRoom(roomCollider.bounds);
        }
    }
}