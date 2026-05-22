using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Rigidbody2D))]
public class DashDirection : MonoBehaviour
{
    [Header("Dash Settings")]
    public float dashSpeed = 15f;
    public int dashLeft = 1;
    public float dashDuration = 0.2f;
    public GameObject Indicator;

    private Rigidbody2D rb;
    private Vector2 dashVector;

    [Header("State")]
    public bool isAiming = false;

    // --- Çapraz Yön Koruması İçin Eklenenler ---
    private const float diagonalGracePeriod = 0.10f; // Çapraz tolere süresi
    private Vector2 lastDiagonalDir = Vector2.zero;
    private float timeSinceLastDiagonal = 999f;
    private Vector2 lockedDir = Vector2.zero;

    public static DashDirection Instance;

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        if (Indicator != null)
            Indicator.SetActive(false);

        rb = GetComponent<Rigidbody2D>();
    }

    private void Update()
    {
        if (dashLeft <= 0)
            return;
        Aim();
    }

    void Aim()
    {
        // 1. START AIMING
        if (Input.GetKeyDown(KeyCode.LeftShift) && !CharacterMovement.Instance.isDashing)
        {
            isAiming = true;
            CharacterMovement.Instance.isAiming = true;
            CharacterMovement.Instance.CanMove = false;

            // Nişan alma başladığında mevcut yönü ilk lockedDir olarak belirle
            lockedDir = GetRawDirectionVector();
            lastDiagonalDir = Vector2.zero;
            timeSinceLastDiagonal = 999f;
        }

        // 2. WHILE AIMING
        if (isAiming)
        {
            if (Indicator != null) Indicator.SetActive(true);

            // Yön inputunu ve çapraz koruma mantığını işle
            ProcessDirectionInput();

            // Indicator'ı sadece geçerli bir yön varsa döndür
            if (lockedDir != Vector2.zero)
            {
                float angle = Mathf.Atan2(lockedDir.y, lockedDir.x) * Mathf.Rad2Deg;
                Indicator.transform.rotation = Quaternion.Euler(0, 0, angle - 90);
            }
        }

        // 3. PERFORM DASH
        if (Input.GetKeyUp(KeyCode.LeftShift) && isAiming)
        {
            isAiming = false;

            if (Indicator != null) Indicator.SetActive(false);

            CharacterMovement.Instance.isAiming = false;

            // Hesaplanan ve filtrelenen son yönü dash yönü olarak belirle
            dashVector = lockedDir;
            dashLeft--;
            StartCoroutine(PerformDash());
        }
    }

    private void ProcessDirectionInput()
    {
        // Enum'dan anlık yönü al
        Vector2 currentDir = GetRawDirectionVector();

        // Eğer bir yöne basılıyorsa işle (Tuş bırakıldıysa eski lockedDir korunur)
        if (currentDir != Vector2.zero)
        {
            // Her iki eksende de hareket varsa bu bir çapraz input'tur
            bool isDiagonal = Mathf.Abs(currentDir.x) > 0.01f && Mathf.Abs(currentDir.y) > 0.01f;

            if (isDiagonal)
            {
                lastDiagonalDir = currentDir;
                timeSinceLastDiagonal = 0f;
            }
            else
            {
                // Dash sırasında zaman yavaşlaması (slow-mo) ihtimaline karşı unscaledDeltaTime
                timeSinceLastDiagonal += Time.unscaledDeltaTime; 
            }

            // ÇAPRAZ KORUMA: Eğer şu an tek bir yöndeysek ama çok kısa süre önce çaprazdaysak, çaprazı koru
            if (!isDiagonal && lastDiagonalDir != Vector2.zero && timeSinceLastDiagonal < diagonalGracePeriod)
            {
                lockedDir = lastDiagonalDir;
            }
            else
            {
                lockedDir = currentDir;
            }
        }
        else
        {
            // Tuşlara basılmıyorsa sadece sayacı artır
            timeSinceLastDiagonal += Time.unscaledDeltaTime;
        }
    }

    private IEnumerator PerformDash()
    {
        CharacterMovement.Instance.CanMove = false;
        CharacterMovement.Instance.isDashing = true;

        // dashVector artık Shift bırakıldığı anda lockedDir'den alınıyor
        if (dashVector == Vector2.zero)
        {
            CharacterMovement.Instance.isDashing = false;
            CharacterMovement.Instance.CanMove = true;
            yield break;
        }

        float elapsedTime = 0f;
        while (elapsedTime < dashDuration)
        {
            rb.linearVelocity = dashVector * dashSpeed;
            elapsedTime += Time.unscaledDeltaTime;
            yield return null;
        }

        rb.linearVelocity = Vector2.zero;
        CharacterMovement.Instance.isDashing = false;
        CharacterMovement.Instance.CanMove = true;
    }

    // --- HELPER METHOD ---
    // Artık sadece Enum'dan o anki raw (ham) yönü çekmek için kullanılıyor.
    private Vector2 GetRawDirectionVector()
    {
        return CharacterMovement.Instance.direction switch
        {
            CharacterMovement.Direction.Up => Vector2.up,
            CharacterMovement.Direction.Down => Vector2.down,
            CharacterMovement.Direction.Left => Vector2.left,
            CharacterMovement.Direction.Right => Vector2.right,
            CharacterMovement.Direction.upLeft => new Vector2(-1, 1).normalized,
            CharacterMovement.Direction.upRight => new Vector2(1, 1).normalized,
            CharacterMovement.Direction.downLeft => new Vector2(-1, -1).normalized,
            CharacterMovement.Direction.downRight => new Vector2(1, -1).normalized,
            _ => Vector2.zero
        };
    }
}