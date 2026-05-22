using Unity.VisualScripting;
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

    void Update()
    {
        if (enemyType == EnemyType.Shooter)
            //Invoke Raycasting to look direction and shoot if in range
             Debug.Log("Shooter enemy looking for player...");
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

        if (CharacterScript.stamina < CharacterScript.baseStamina)
            CharacterScript.stamina = CharacterScript.baseStamina;

        DashDirection.Instance.dashLeft = 1;
    }
}