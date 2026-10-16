using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class BeeController : MonoBehaviour
{
    public float moveSpeed = 6f;
    [SerializeField] private float screenMargin = 0.25f;

    [Header("Vertical Edge Push")]
    [SerializeField] private float verticalPushZone = 1.2f;
    [SerializeField] private float minimumPushSpeed = 2.5f;
    [SerializeField] private float maximumPushSpeed = 9f;

    [Header("Animation")]
    [SerializeField] private Animator animator;
    [SerializeField] private SpriteRenderer spriteRenderer;

    private Rigidbody2D rb;
    private Vector2 input;
    private BeeHealth beeHealth;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        rb.gravityScale = 0f;
        rb.constraints = RigidbodyConstraints2D.FreezeRotation;

        beeHealth = GetComponent<BeeHealth>();
    }

    private void Update()
    {
        if (!GameFlow.I || !GameFlow.I.IsRunning || GameFlow.I.IsGameOver)
        {
            input = Vector2.zero;
            UpdateAnimation();
            return;
        }

        float x = Input.GetAxisRaw("Horizontal");
        float y = Input.GetAxisRaw("Vertical");

        input = new Vector2(x, y).normalized;

        UpdateAnimation();
    }

    private void FixedUpdate()
    {
        // 기본 WASD 이동 속도
        Vector2 targetVelocity = input * moveSpeed;

        // 화면 상단과 하단에서 중심 방향으로 속도 교정
        targetVelocity = ApplyVerticalEdgePush(targetVelocity);

        rb.linearVelocity = targetVelocity;

        // 좀비가 화면 밖으로 완전히 나가는 것 방지
        if (CameraBounds2D.I != null)
        {
            Vector2 clampedPosition =
                CameraBounds2D.I.ClampInside(
                    rb.position,
                    -screenMargin
                );

            rb.position = clampedPosition;
        }
    }

    private Vector2 ApplyVerticalEdgePush(Vector2 velocity)
    {
        if (CameraBounds2D.I == null)
        {
            return velocity;
        }

        // 실제로 좀비가 이동할 수 있는 화면 내부 영역
        Rect screenRect =
            CameraBounds2D.I.GetWorldRect(-screenMargin);

        // 위험구역이 화면 절반보다 커지는 것을 방지
        float zoneSize = Mathf.Min(
            verticalPushZone,
            screenRect.height * 0.45f
        );

        float topPushStart =
            screenRect.yMax - zoneSize;

        float bottomPushStart =
            screenRect.yMin + zoneSize;

        float currentY = rb.position.y;

        // 화면 상단 위험구역
        if (currentY >= topPushStart)
        {
            // 0: 위험구역 입구
            // 1: 화면 최상단
            float depth = Mathf.InverseLerp(
                topPushStart,
                screenRect.yMax,
                currentY
            );

            float pushSpeed = Mathf.Lerp(
                minimumPushSpeed,
                maximumPushSpeed,
                depth
            );

            // 기존 이동보다 아래쪽 속도가 우선되게 함
            velocity.y = Mathf.Min(
                velocity.y,
                -pushSpeed
            );
        }
        // 화면 하단 위험구역
        else if (currentY <= bottomPushStart)
        {
            // 0: 위험구역 입구
            // 1: 화면 최하단
            float depth = Mathf.InverseLerp(
                bottomPushStart,
                screenRect.yMin,
                currentY
            );

            float pushSpeed = Mathf.Lerp(
                minimumPushSpeed,
                maximumPushSpeed,
                depth
            );

            // 기존 이동보다 위쪽 속도가 우선되게 함
            velocity.y = Mathf.Max(
                velocity.y,
                pushSpeed
            );
        }

        return velocity;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!GameFlow.I || GameFlow.I.IsGameOver) return;

        if (other.CompareTag("Tongue"))
        {
            if (beeHealth != null)
            {
                if (!beeHealth.IsInvincible)
                {
                    beeHealth.TakeDamage(1);
                }
            }
            else
            {
                GameFlow.I.GameOver();
            }
        }
    }
    private void UpdateAnimation()
    {
        bool isMoving = input.sqrMagnitude > 0.01f;

        if (animator != null)
        {
            animator.SetBool("IsMoving", isMoving);
        }

        if (spriteRenderer == null)
        {
            return;
        }

        if (input.x > 0.01f)
        {
            // 오른쪽 입력
            spriteRenderer.flipX = false;
        }
        else if (input.x < -0.01f)
        {
            // 왼쪽 입력
            spriteRenderer.flipX = true;
        }
    }
}
