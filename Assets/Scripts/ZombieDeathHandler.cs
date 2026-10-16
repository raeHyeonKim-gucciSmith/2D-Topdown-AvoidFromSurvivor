using UnityEngine;

public class ZombieDeathHandler : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Animator animator;
    [SerializeField] private BeeController movementController;
    [SerializeField] private Rigidbody2D rb;
    [SerializeField] private Collider2D playerCollider;

    [Header("Animation")]
    [SerializeField] private string deathTriggerName = "Die";

    private bool hasDied;

    public void PlayDeathAnimation()
    {
        if (hasDied)
        {
            return;
        }

        hasDied = true;

        if (GameAudioManager.I != null)
        {
            GameAudioManager.I.PlayZombieDeath();
        }

        // 사망 후 WASD 이동 중지
        if (movementController != null)
        {
            movementController.enabled = false;
        }

        // 현재 이동 속도 정지
        if (rb != null)
        {
            rb.linearVelocity = Vector2.zero;
            rb.angularVelocity = 0f;
        }

        // 사망 후 추가 충돌 방지
        if (playerCollider != null)
        {
            playerCollider.enabled = false;
        }

        if (animator != null)
        {
            animator.SetBool("IsMoving", false);
            animator.ResetTrigger(deathTriggerName);
            animator.SetTrigger(deathTriggerName);
        }
    }
}