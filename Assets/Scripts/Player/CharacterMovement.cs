using System;
using Unity.Mathematics;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(Animator))]
public class CharacterMovement : MonoBehaviour
{
    [Header("Movement Settings")]
    public float moveSpeed = 5f;

    [Header("Stamina Settings")]
    public float baseStamina = 3f;
    public float stamina = 3f;
    public float distanceTraveled;

    [Header("Time Settings")]
    public float timeLerpSpeed = 10f;
    public float minTimeScale = 0.05f;

    [Header("SFX")]
    public GameObject[] FootstepClips;

    private Rigidbody2D rb;
    private Animator anim;
    private Vector2 movement;
    private Vector3 lastPosition;

    private Vector2 lastFacingDirection = new Vector2(0, -1);

    public static CharacterMovement Instance;

    [Header("States")]
    public bool CanMove = true;
    public bool isDashing = false;
    public bool isAiming = false;

    private SpriteRenderer sr;
    float MaxStaminaAchieved = 0f;

    private Sprite lastSprite;
    private int spriteFrameCounter = 0;

    public enum Direction
    {
        Up, Down, Left, Right, upLeft, upRight, downLeft, downRight
    }
    public Direction direction;

    public enum State
    {
        Normal, Dashing, Aiming, moving
    }
    public State state;

    private void Awake()
    {
        Instance = this;
    }

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        sr = GetComponent<SpriteRenderer>();
        lastPosition = transform.position;

        lastSprite = sr.sprite;
    }

    void Update()
    {
        movement.x = Input.GetAxisRaw("Horizontal");
        movement.y = Input.GetAxisRaw("Vertical");

        if (movement.sqrMagnitude > 1f)
        {
            movement.Normalize();
        }

        InputDirection();
        UpdateState();
        LerpTime();

        UpdateAnimations();
        UpdateUI();
    }

    void FixedUpdate()
    {
        CalculateDistance();

        if (stamina > 0 && CanMove)
            MoveCharacter();
    }

    void UpdateUI()
    {
        if (LevelManager.Instance == null) return;

        if (stamina > MaxStaminaAchieved)
        {
            MaxStaminaAchieved = stamina;
        }

        if (LevelManager.Instance.StaminaSlider != null && MaxStaminaAchieved > 0f)
        {
            float targetValue = stamina / MaxStaminaAchieved;
            float lerpSpeed = 10f;

            LevelManager.Instance.StaminaSlider.value = Mathf.Lerp(
                LevelManager.Instance.StaminaSlider.value,
                targetValue,
                lerpSpeed * Time.unscaledDeltaTime
            );
        }

        if (LevelManager.Instance.DashText != null && DashDirection.Instance != null)
        {
            LevelManager.Instance.DashText.text = $"Dash Left: {DashDirection.Instance.dashLeft}";
        }
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

        if (isDashing)
        {
            targetTimeScale = 1f;
        }
        else if (isAiming)
        {
            targetTimeScale = minTimeScale;
        }
        else if (movement.sqrMagnitude > 0.01f && stamina > 0)
        {
            targetTimeScale = 1f;
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

    void UpdateState()
    {
        if (isDashing)
        {
            state = State.Dashing;
        }
        else if (isAiming)
        {
            state = State.Aiming;
        }
        else if (movement.sqrMagnitude > 0.01f && stamina > 0f)
        {
            state = State.moving;
        }
        else
        {
            state = State.Normal;
        }
    }

    void UpdateAnimations()
    {
        if (anim == null) return;

        if (movement.sqrMagnitude > 0.01f)
        {
            lastFacingDirection = movement.normalized;
        }

        if (movement.x > 0)
        {
            sr.flipX = false;
        }
        else if (movement.x < 0)
        {
            sr.flipX = true;
        }

        anim.SetFloat("MoveX", movement.x);
        anim.SetFloat("MoveY", movement.y);
        anim.SetFloat("LastMoveX", lastFacingDirection.x);
        anim.SetFloat("LastMoveY", lastFacingDirection.y);

        anim.SetBool("IsMoving", state == State.moving);
        anim.SetBool("IsDashing", state == State.Dashing);
        anim.SetBool("IsAiming", state == State.Aiming);

        if (state == State.moving)
        {
            if (sr.sprite != lastSprite)
            {
                lastSprite = sr.sprite;
                spriteFrameCounter++;

                if (spriteFrameCounter >= 4)
                {
                    Footstep();
                    spriteFrameCounter = 0;
                }
            }
        }
        else
        {
            lastSprite = sr.sprite;
            spriteFrameCounter = 0;
        }
    }

    public void Die()
    {
        CanMove = false;
        anim.SetTrigger("Die");
        LevelManager.Instance.ReloadScene();
    }

    public void Footstep()
    {
        GameObject clipToPlay = FootstepClips[UnityEngine.Random.Range(0, FootstepClips.Length)];
        var obj = Instantiate(clipToPlay, transform.position, quaternion.identity);
        Destroy(obj, 1);
    }
}