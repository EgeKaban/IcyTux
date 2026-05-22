using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(CapsuleCollider2D))]
public class EnemyBullet : MonoBehaviour
{
    public float speed = 15f;
    public float lifetime = 5f;
    private Rigidbody2D rb;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        rb.linearVelocity = transform.up * speed; // Move in the direction the firePoint is facing
        Destroy(gameObject, lifetime); // Destroy bullet after its lifetime expires
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            // Handle damage to player here (e.g., call a method on the player's health script)
            // Example: collision.GetComponent<PlayerHealth>().TakeDamage(damageAmount);
        }
        // Destroy bullet on any collision (you can add exceptions if needed)
        Destroy(gameObject);
    }
}