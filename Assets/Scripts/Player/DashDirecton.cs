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
    public AudioClip dashSound;

    [Header("SFX")]
    public GameObject DashSFX;

    private Rigidbody2D rb;
    private Vector2 dashVector;

    [Header("State")]
    public bool isAiming = false;

    private const float diagonalGracePeriod = 0.10f;
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
        if (LevelManager.Instance.isLoading)
            return;

        if (Input.GetKeyDown(KeyCode.LeftShift) && !CharacterMovement.Instance.isDashing)
        {
            isAiming = true;
            CharacterMovement.Instance.isAiming = true;
            CharacterMovement.Instance.CanMove = false;

            lockedDir = GetRawDirectionVector();
            lastDiagonalDir = Vector2.zero;
            timeSinceLastDiagonal = 999f;
        }

        if (isAiming)
        {
            if (Indicator != null) Indicator.SetActive(true);

            ProcessDirectionInput();

            if (lockedDir != Vector2.zero)
            {
                float angle = Mathf.Atan2(lockedDir.y, lockedDir.x) * Mathf.Rad2Deg;
                Indicator.transform.rotation = Quaternion.Euler(0, 0, angle - 90);
            }
        }

        if (Input.GetKeyUp(KeyCode.LeftShift) && isAiming)
        {
            isAiming = false;

            if (Indicator != null) Indicator.SetActive(false);

            CharacterMovement.Instance.isAiming = false;

            dashVector = lockedDir;
            dashLeft--;
            if (dashLeft == 0)
            {
                if (LevelManager.Instance != null)
                {
                    LevelManager.Instance.CheckLastDash();
                }
            }
            StartCoroutine(PerformDash());
        }
    }

    private void ProcessDirectionInput()
    {
        Vector2 currentDir = GetRawDirectionVector();

        if (currentDir != Vector2.zero)
        {
            bool isDiagonal = Mathf.Abs(currentDir.x) > 0.01f && Mathf.Abs(currentDir.y) > 0.01f;

            if (isDiagonal)
            {
                lastDiagonalDir = currentDir;
                timeSinceLastDiagonal = 0f;
            }
            else
            {
                timeSinceLastDiagonal += Time.unscaledDeltaTime; 
            }

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
            timeSinceLastDiagonal += Time.unscaledDeltaTime;
        }
    }

    private IEnumerator PerformDash()
    {
        CharacterMovement.Instance.CanMove = false;
        CharacterMovement.Instance.isDashing = true;

        if (RoomCamera.Instance != null)
        {
            RoomCamera.Instance.TriggerZoomEffect(1f, dashDuration, 20f);
        }

        if (dashVector == Vector2.zero)
        {
            CharacterMovement.Instance.isDashing = false;
            CharacterMovement.Instance.CanMove = true;
            yield break;
        }

        if (dashSound != null && LevelManager.Instance != null)
        {
            var obj = Instantiate(DashSFX, transform.position, Quaternion.identity);
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