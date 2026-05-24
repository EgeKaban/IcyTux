using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(Animator))]
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

    // --- EKLENEN SİLAH DEĞİŞKENİ ---
    [Header("Weapon Settings")]
    public Transform weaponPivot;

    private float nextFireTime = 0f;

    // --- ANİMASYON VE YÖN DEĞİŞKENLERİ ---
    private Animator anim;
    private SpriteRenderer sr;
    private Rigidbody2D rb;
    private Vector2 currentFacingDirection = Vector2.down; // Başlangıç yönü
    private CircleCollider2D selfCollider;

    private void Awake()
    {
        selfCollider = GetComponent<CircleCollider2D>();
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

        // Animasyonları ve silah yönünü her frame güncelle
        UpdateAnimations();
        AimWeapon(); // EKLENDİ
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
        selfCollider.enabled = false;
        Debug.Log("Enemy died!");

        if (LevelManager.Instance != null)
        {
            LevelManager.Instance.RegisterEnemyDeath();
        }

        if (restoreMaxStamina)
        {
            CharacterScript.stamina = CharacterScript.baseStamina;
        }
        else
        {
            CharacterScript.stamina += staminaGiven;
        }

        DashDirection.Instance.dashLeft += dashesGiven;
        
        anim.SetTrigger("Die");
    }

    public void Destroy()
    {
        Destroy(gameObject);
    }

    // --- SİLAH NİŞAN ALMA METODU (EKLENDİ) ---
    private void AimWeapon()
    {
        // Eğer silah pivotu atanmamışsa veya düşman Shooter değilse işlem yapma
        if (weaponPivot == null || enemyType != EnemyType.Shooter) return;

        // 1. Bakış yönüne göre açıyı hesapla
        float angle = Mathf.Atan2(currentFacingDirection.y, currentFacingDirection.x) * Mathf.Rad2Deg;

        // 2. Pivotu döndür
        weaponPivot.rotation = Quaternion.Euler(0, 0, angle);

        // 3. Sola bakarken silahın ters (amuda kalkmış) görünmemesi için Y ekseninde çevir
        if (Mathf.Abs(angle) > 90f)
        {
            weaponPivot.localScale = new Vector3(1f, -1f, 1f); // Sola bakıyor
        }
        else
        {
            weaponPivot.localScale = new Vector3(1f, 1f, 1f);  // Sağa bakıyor
        }
    }

    private void Shoot()
    {
        if (bulletPrefab != null && firePoint != null)
        {
            // Silah pivotu zaten doğru yöne döndüğü için ve firePoint de onun alt objesi olduğu için,
            // doğrudan firePoint'in rotasyonunu kullanabiliriz.
            // Merminin dik (Up) ekseninin ileri bakması için Z ekseninde -90 derece ekliyoruz.
            Quaternion bulletRotation = firePoint.rotation * Quaternion.Euler(0, 0, -90f);

            CameraShake.Instance.Shake(0.1f, 0.1f);
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

        // Sola doğru bakıyorsa (X ekseninde - değere gidiyorsa) Düşman Sprite'ını çevir (Flip)
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