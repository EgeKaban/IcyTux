using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class CharacterMovement : MonoBehaviour
{
    [Header("Movement Settings")]
    public float moveSpeed = 5f;

    [Header("Stamina Settings")]
    public float maxStamina = 3f;
    public float stamina = 3f;
    public float distanceTraveled;

    [Header("Time Settings")]
    public float timeLerpSpeed = 10f;
    public float minTimeScale = 0.05f;

    private Rigidbody2D rb;
    private Vector2 movement;
    private Vector3 lastPosition;

    public static CharacterMovement Instance;

    [Header("States")]
    public bool CanMove = true;
    public bool isDashing = false;
    public bool isAiming = false; // Added to let the time system know we are aiming

    public enum Direction
    {
        Up, Down, Left, Right, upLeft, upRight, downLeft, downRight
    }
    public Direction direction;

    private void Awake()
    {
        Instance = this;
    }

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        lastPosition = transform.position;
    }

    void Update()
    {
        // FIX: Always read input so we know which way the player WANTS to go.
        // We will stop the actual physical movement in FixedUpdate instead.
        movement.x = Input.GetAxisRaw("Horizontal");
        movement.y = Input.GetAxisRaw("Vertical");

        if (movement.sqrMagnitude > 1f)
        {
            movement.Normalize();
        }

        InputDirection();

        LerpTime();
    }

    void FixedUpdate()
    {
        CalculateDistance();

        // FIX: Only physically move the character if CanMove is true
        if (stamina > 0 && CanMove)
            MoveCharacter();
    }

    void MoveCharacter()
    {
        rb.MovePosition(rb.position + movement * moveSpeed * Time.fixedDeltaTime);
    }

    void CalculateDistance()
    {
        float frameDistance = Vector3.Distance(lastPosition, transform.position);

        if (frameDistance > 0 && stamina > 0 && !isDashing)
        {
            stamina -= frameDistance;
            distanceTraveled += frameDistance;
        }

        lastPosition = transform.position;
    }

    void LerpTime()
    {
        float targetTimeScale = minTimeScale;

        // FIX: Determine time scale based on state priorities
        if (isDashing)
        {
            targetTimeScale = 1f; // Fast time while dashing
        }
        else if (isAiming)
        {
            targetTimeScale = minTimeScale; // Slow time while aiming (even if pressing WASD)
        }
        else if (movement.sqrMagnitude > 0.01f && stamina > 0)
        {
            targetTimeScale = 1f; // Fast time while moving normally
        }

        float lerpFactor = 1f - Mathf.Exp(-timeLerpSpeed * Time.unscaledDeltaTime);
        Time.timeScale = Mathf.Lerp(Time.timeScale, targetTimeScale, lerpFactor);

        Time.fixedDeltaTime = 0.02f * Time.timeScale;
    }

    void InputDirection()
    {
        if (movement.x > 0 && movement.y > 0) direction = Direction.upRight;
        else if (movement.x < 0 && movement.y > 0) direction = Direction.upLeft;
        else if (movement.x > 0 && movement.y < 0) direction = Direction.downRight;
        else if (movement.x < 0 && movement.y < 0) direction = Direction.downLeft;
        else if (movement.x > 0) direction = Direction.Right;
        else if (movement.x < 0) direction = Direction.Left;
        else if (movement.y > 0) direction = Direction.Up;
        else if (movement.y < 0) direction = Direction.Down;
    }
}