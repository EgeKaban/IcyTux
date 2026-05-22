using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
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
    public float staminaGiven = 10f; // Hidden in Inspector if restoreMaxStamina is true
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
    }

    private void Awake()
    {
        Rigidbody2D rb = GetComponent<Rigidbody2D>();
        rb.gravityScale = 0f;
        rb.constraints = RigidbodyConstraints2D.FreezeRotation;
        rb.collisionDetectionMode = CollisionDetectionMode2D.Continuous;
        rb.interpolation = RigidbodyInterpolation2D.Interpolate;
    }

    private void Start()
    {
        CharacterScript = CharacterMovement.Instance;
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

        // Stamina Logic
        if (restoreMaxStamina)
        {
            // Restore to exactly max base stamina
            CharacterScript.stamina = CharacterScript.baseStamina;
        }
        else
        {
            // Add custom stamina, clamp so it doesn't exceed base
            CharacterScript.stamina += staminaGiven;
            if (CharacterScript.stamina > CharacterScript.baseStamina)
            {
                CharacterScript.stamina = CharacterScript.baseStamina;
            }
        }

        DashDirection.Instance.dashLeft += dashesGiven;
        Destroy(gameObject);
    }

    private void Shoot()
    {
        if (bulletPrefab != null && firePoint != null)
        {
            // Rotate -90 on Z axis so the bullet's Up (green) axis faces outward from the gun
            Quaternion bulletRotation = transform.rotation * Quaternion.Euler(0, 0, -90f);

            // Spawn it! EnemyBullet.cs handles the movement automatically.
            Instantiate(bulletPrefab, firePoint.position, bulletRotation);
        }
    }

    private void TrackPlayer8Way()
    {
        if (CharacterScript == null) return;

        Vector2 directionToPlayer = CharacterScript.transform.position - transform.position;
        float rawAngle = Mathf.Atan2(directionToPlayer.y, directionToPlayer.x) * Mathf.Rad2Deg;
        float snappedAngle = Mathf.Round(rawAngle / 45f) * 45f;
        transform.rotation = Quaternion.Euler(0, 0, snappedAngle);
    }

    private bool CheckVisionCone()
    {
        if (CharacterScript == null) return false;

        Vector2 origin = transform.position;
        Vector2 facingDirection = transform.right;
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

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Vector2 origin = transform.position;
        Vector2 facingDirection = transform.right;
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