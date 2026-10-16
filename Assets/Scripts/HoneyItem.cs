using System.Collections;
using UnityEngine;

public class HoneyItem : MonoBehaviour
{
    [Header("Pickup Settings")]
    [SerializeField] private int healAmount = 1;

    [Header("Full Health Push")]
    [SerializeField] private float pushSpeed = 8f;
    [SerializeField] private float pushDuration = 0.2f;

    [Header("Animation")]
    [SerializeField] private Animator humanAnimator;

    [Header("Visual")]
    [SerializeField] private GameObject pickupEffect;

    private bool isPushing;

    // Animator에 만든 Trigger 이름
    private static readonly int PushHash =
        Animator.StringToHash("Push");

    private void Awake()
    {
        // Inspector에서 연결하지 않았을 경우 자동으로 찾음
        if (humanAnimator == null)
        {
            humanAnimator = GetComponent<Animator>();
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        // 충돌한 대상이 좀비가 아니면 무시
        if (!other.CompareTag("Player"))
        {
            return;
        }

        BeeHealth beeHealth = other.GetComponent<BeeHealth>();

        if (beeHealth == null)
        {
            return;
        }

        bool healed = beeHealth.Heal(healAmount);

        if (healed)
        {
            if (GameAudioManager.I != null)
            {
                GameAudioManager.I.PlayHeal();
            }

            // 체력 회복 성공 시 먹는 효과 생성
            if (pickupEffect != null)
            {
                Instantiate(
                    pickupEffect,
                    transform.position,
                    Quaternion.identity
                );
            }

            // 체력 회복에 성공했을 때만 Human 제거
            Destroy(gameObject);
            return;
        }

        // 게임오버 상태에서는 밀어내지 않음
        if (GameFlow.I != null && GameFlow.I.IsGameOver)
        {
            return;
        }

        // 최대 체력이면 Human을 유지하고 좀비 밀어내기
        if (beeHealth.GetCurrentHealth() >= beeHealth.GetMaxHealth()
            && !isPushing)
        {
            StartCoroutine(PushPlayer(other));
        }
    }

    private IEnumerator PushPlayer(Collider2D playerCollider)
    {
        isPushing = true;


        Rigidbody2D playerRb = playerCollider.attachedRigidbody;
        BeeController controller =
            playerCollider.GetComponent<BeeController>();

        if (playerRb == null)
        {
            isPushing = false;
            yield break;
        }

        // Human과 좀비 위치 확인
        Vector2 humanPosition = transform.position;
        Vector2 playerPosition = playerCollider.transform.position;

        // Human에서 좀비 방향으로 미는 방향 계산
        Vector2 pushDirection =
            (playerPosition - humanPosition).normalized;

        // 두 오브젝트의 중심이 거의 완전히 겹쳤을 경우
        if (pushDirection.sqrMagnitude < 0.001f)
        {
            pushDirection = Vector2.right;
        }

        // Push Trigger 실행
        // Animator가 human 상태에서 PushZombie 상태로 전환됨
        if (humanAnimator != null)
        {
            humanAnimator.ResetTrigger(PushHash);
            humanAnimator.SetTrigger(PushHash);
        }

        if (GameAudioManager.I != null)
        {
            GameAudioManager.I.PlayPush();
        }

        // 플레이어 이동이 밀어내기 속도를 덮어쓰지 않도록 잠시 정지
        if (controller != null)
        {
            controller.enabled = false;
        }

        // 좀비 밀어내기
        playerRb.linearVelocity = pushDirection * pushSpeed;

        yield return new WaitForSeconds(pushDuration);

        // 밀어내기 종료
        playerRb.linearVelocity = Vector2.zero;

        // 게임오버가 아니라면 조작 복구
        if (controller != null
            && (GameFlow.I == null || !GameFlow.I.IsGameOver))
        {
            controller.enabled = true;
        }

        isPushing = false;
    }
}