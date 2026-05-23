using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(Animator))] // Animator bileşeni eklendi
public class Enemy : MonoBehaviour
{
    CharacterMovement CharacterScript;

    public EnemyType enemyType;
    public enum EnemyType
    {
        Normal,
        Shooter,
        Elite
    }

    [Header("Rewards")]
    public bool restoreMaxStamina = true;
    public float staminaGiven = 10f;
    public int dashesGiven = 1;

    [Header("Stealth Settings (Cone)")]
    public float visionDistance = 10f;
    [Range(0f, 360f)]
    public float coneAngle = 60f;
    [Range(1, 20)]
    public int visionResolution = 10;
    public LayerMask visionMask;

    [Header("Shooting Settings")]
    public GameObject bulletPrefab;
    public Transform firePoint;
    public float fireDelay = 1.5f;

    private float nextFireTime = 0f;

    // --- ANİMASYON VE YÖN DEĞİŞKENLERİ ---
    private Animator anim;
    private SpriteRenderer sr;
    private Rigidbody2D rb;
    private Vector2 currentFacingDirection = Vector2.down; // Başlangıç yönü

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        rb.gravityScale = 0f;
        rb.constraints = RigidbodyConstraints2D.FreezeRotation; // Z ekseninde dönmeyi kilitledik
        rb.collisionDetectionMode = CollisionDetectionMode2D.Continuous;
        rb.interpolation = RigidbodyInterpolation2D.Interpolate;

        anim = GetComponent<Animator>();
        sr = GetComponent<SpriteRenderer>();
    }

    private void Start()
    {
        CharacterScript = CharacterMovement.Instance;
    }

    void Update()
    {
        if (enemyType == EnemyType.Shooter)
        {
            if (CheckVisionCone())
            {
                TrackPlayer8Way();

                if (Time.time >= nextFireTime)
                {
                    Shoot();
                    nextFireTime = Time.time + fireDelay;
                }
            }
        }
        else
        {
            // Eğer hareket eden (Normal/Elite) bir düşmansa ve bir hızı varsa yönünü hıza göre belirle
            if (rb.linearVelocity.sqrMagnitude > 0.01f)
            {
                currentFacingDirection = rb.linearVelocity.normalized;
            }
        }

        // Animasyonları her frame güncelle
        UpdateAnimations();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player") && CharacterScript.state == CharacterMovement.State.Dashing)
        {
            Die();
        }
        else if (collision.gameObject.CompareTag("Player") && CharacterScript.state != CharacterMovement.State.Dashing)
        {
            CharacterScript.Die();
        }
    }

    void Die()
    {
        Debug.Log("Enemy died!");

        if (restoreMaxStamina)
        {
            CharacterScript.stamina = CharacterScript.baseStamina;
        }
        else
        {
            CharacterScript.stamina += staminaGiven;
        }

        DashDirection.Instance.dashLeft += dashesGiven;
        Destroy(gameObject);
    }

    private void Shoot()
    {
        if (bulletPrefab != null && firePoint != null)
        {
            // Merminin rotasyonunu Transform yerine currentFacingDirection vektörüne göre hesaplıyoruz
            float angle = Mathf.Atan2(currentFacingDirection.y, currentFacingDirection.x) * Mathf.Rad2Deg;

            // Merminin Up (Yeşil) ekseninin dışarı bakması için -90 derece ekliyoruz
            Quaternion bulletRotation = Quaternion.Euler(0, 0, angle - 90f);

            Instantiate(bulletPrefab, firePoint.position, bulletRotation);
        }
    }

    private void TrackPlayer8Way()
    {
        if (CharacterScript == null) return;

        Vector2 directionToPlayer = CharacterScript.transform.position - transform.position;
        float rawAngle = Mathf.Atan2(directionToPlayer.y, directionToPlayer.x) * Mathf.Rad2Deg;

        // 8 yöne (45 derecelik açılara) yuvarla
        float snappedAngle = Mathf.Round(rawAngle / 45f) * 45f;
        float rad = snappedAngle * Mathf.Deg2Rad;

        // Transform'u döndürmek yerine sadece bakış yönü vektörünü güncelliyoruz
        currentFacingDirection = new Vector2(Mathf.Cos(rad), Mathf.Sin(rad)).normalized;
    }

    private bool CheckVisionCone()
    {
        if (CharacterScript == null) return false;

        Vector2 origin = transform.position;
        Vector2 facingDirection = currentFacingDirection; // transform.right YERİNE yön vektörümüzü kullanıyoruz
        Vector2 directionToPlayer = (Vector2)CharacterScript.transform.position - origin;
        float distanceToPlayer = directionToPlayer.magnitude;

        if (distanceToPlayer <= visionDistance)
        {
            float angleToPlayer = Vector2.Angle(facingDirection, directionToPlayer);
            if (angleToPlayer <= coneAngle / 2f)
            {
                float selfCollisionBuffer = 0.1f;
                Vector2 rayOrigin = origin + facingDirection.normalized * selfCollisionBuffer;

                RaycastHit2D hit = Physics2D.Raycast(rayOrigin, directionToPlayer.normalized, distanceToPlayer, visionMask);

                if (hit.collider != null && hit.collider.CompareTag("Player"))
                {
                    return true;
                }
            }
        }
        return false;
    }

    private void UpdateAnimations()
    {
        if (anim == null) return;

        // Sola doğru bakıyorsa (X ekseninde - değere gidiyorsa) Sprite'ı çevir (Flip)
        if (currentFacingDirection.x > 0.1f)
        {
            if (sr != null) sr.flipX = false;
        }
        else if (currentFacingDirection.x < -0.1f)
        {
            if (sr != null) sr.flipX = true;
        }

        // Blend Tree için parametreleri gönder
        anim.SetFloat("MoveX", currentFacingDirection.x);
        anim.SetFloat("MoveY", currentFacingDirection.y);

        // Eğer düşmanın bir hızı varsa hareket ediyor demektir
        bool isMoving = rb.linearVelocity.sqrMagnitude > 0.01f;
        //anim.SetBool("IsMoving", isMoving);
    }

    private void OnDrawGizmosSelected()
    {
        if (enemyType != EnemyType.Shooter) return;

        Gizmos.color = Color.yellow;
        Vector2 origin = transform.position;

        // Edit modunda (oyun oynanmıyorken) currentFacingDirection güncellenmeyebileceği için transform.right yedeği eklendi
        Vector2 facingDirection = Application.isPlaying ? currentFacingDirection : (Vector2)transform.right;

        Gizmos.DrawLine(origin, origin + facingDirection * visionDistance);

        float stepAngle = coneAngle / visionResolution;
        for (int i = 0; i <= visionResolution; i++)
        {
            float currentAngle = -coneAngle / 2f + i * stepAngle;
            Vector2 rayDirection = Quaternion.Euler(0, 0, currentAngle) * facingDirection;
            Vector2 rayEnd = origin + rayDirection * visionDistance;
            Gizmos.DrawLine(origin, rayEnd);
        }

        Vector2 previousBasePoint = origin + (Vector2)(Quaternion.Euler(0, 0, -coneAngle / 2f) * facingDirection) * visionDistance;
        for (int i = 1; i <= visionResolution; i++)
        {
            float currentAngle = -coneAngle / 2f + i * stepAngle;
            Vector2 currentBasePoint = origin + (Vector2)(Quaternion.Euler(0, 0, currentAngle) * facingDirection) * visionDistance;
            Gizmos.DrawLine(previousBasePoint, currentBasePoint);
            previousBasePoint = currentBasePoint;
        }
    }
}