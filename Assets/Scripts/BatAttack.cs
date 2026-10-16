using System.Collections;
using UnityEngine;

public class BatAttack : MonoBehaviour
{
    [Header("Timing")]
    [SerializeField] private float hitVisibleTime = 0.25f;

    [Header("Damage")]
    [SerializeField] private int damage = 1;
    [SerializeField] private LayerMask playerLayer;
    [SerializeField] private float attackerHitRadius = 0.8f;

    [Header("Warning Line")]
    [SerializeField] private LineRenderer warningLine;
    [SerializeField] private float warningGrowTime = 1f;
    [SerializeField] private float lineWidth = 0.45f;

    [Header("Moving Attacker")]
    [SerializeField] private GameObject movingAttacker;
    [SerializeField] private SpriteRenderer attackerRenderer;
    [SerializeField] private Animator attackerAnimator;
    [SerializeField] private float attackerEdgeOffset = 0.5f;
    [SerializeField] private float lineEndGap = 0.35f;
    [SerializeField] private float returnDuration = 1.2f;
    [Range(0f, 1f)]
    [SerializeField] private float minimumTravelRatio = 0.333f;

    private Vector2 attackStartPoint;
    private Vector2 attackEndPoint;
    private Vector2 characterStartPosition;

    private bool hitDuringForward;
    private bool hitDuringReturn;

    private void Start()
    {
        StartCoroutine(AttackRoutine());
    }

    private IEnumerator AttackRoutine()
    {
        // 새로운 공격이 시작될 때 피해 여부 초기화
        hitDuringForward = false;
        hitDuringReturn = false;

        if (movingAttacker != null)
        {
            movingAttacker.SetActive(false);
        }

        // 캐릭터가 전진하며 최대 한 번 피해
        yield return StartCoroutine(GrowWarningLine());

        // 끝 지점에서 잠시 멈춘다.
        yield return new WaitForSeconds(hitVisibleTime);

        // 캐릭터가 복귀하며 최대 한 번 추가 피해
        yield return StartCoroutine(RetractWarningLine());

        if (warningLine != null)
        {
            warningLine.gameObject.SetActive(false);
        }

        Destroy(gameObject);
    }

    private IEnumerator GrowWarningLine()
    {
        if (warningLine == null)
        {
            yield return new WaitForSeconds(warningGrowTime);
            yield break;
        }

        warningLine.gameObject.SetActive(true);
        warningLine.positionCount = 2;
        warningLine.startWidth = lineWidth;
        warningLine.endWidth = lineWidth;

        attackEndPoint = transform.position;
        attackStartPoint = GetStartPointFromScreenEdge(attackEndPoint);

        // 목표가 가장자리와 너무 가까우면 화면 1/3 지점까지 보정
        ClampEndPointToMinimumDistance();

        Vector2 lineDirection =
            (attackEndPoint - attackStartPoint).normalized;

        characterStartPosition =
            attackStartPoint - lineDirection * attackerEdgeOffset;

        if (movingAttacker != null)
        {
            movingAttacker.transform.position = characterStartPosition;
            movingAttacker.SetActive(true);
        }

        SetAttackerDirection(lineDirection);

        if (attackerAnimator != null)
        {
            attackerAnimator.enabled = true;
            attackerAnimator.Play("BatSwing", 0, 0f);
        }

        float timer = 0f;

        while (timer < warningGrowTime)
        {
            float t = timer / warningGrowTime;

            Vector2 currentPosition = Vector2.Lerp(
                characterStartPosition,
                attackEndPoint,
                t
            );

            // 캐릭터 이동
            if (movingAttacker != null)
            {
                movingAttacker.transform.position = currentPosition;
            }

            // 전진 중에는 최대 한 번 피해
            if (!hitDuringForward)
            {
                hitDuringForward = TryHitPlayerAt(currentPosition);
            }

            // 캐릭터 뒤까지 경고선 표시
            Vector2 visualLineEnd =
                currentPosition - lineDirection * lineEndGap;

            warningLine.SetPosition(0, attackStartPoint);
            warningLine.SetPosition(1, visualLineEnd);

            timer += Time.deltaTime;
            yield return null;
        }

        // 마지막 위치를 정확하게 고정
        if (movingAttacker != null)
        {
            movingAttacker.transform.position = attackEndPoint;
        }

        warningLine.SetPosition(0, attackStartPoint);
        warningLine.SetPosition(
            1,
            attackEndPoint - lineDirection * lineEndGap
        );
    }
    private IEnumerator RetractWarningLine()
    {
        if (movingAttacker == null || warningLine == null)
        {
            yield break;
        }

        Vector2 lineDirection =
            (attackEndPoint - attackStartPoint).normalized;

        Vector2 fullLineEnd =
            attackEndPoint - lineDirection * lineEndGap;

        float timer = 0f;

        while (timer < returnDuration)
        {
            float t = timer / returnDuration;

            // 캐릭터가 목표 지점에서 처음 출발 위치로 돌아간다.
            Vector2 currentPosition = Vector2.Lerp(
                attackEndPoint,
                characterStartPosition,
                t
            );

            movingAttacker.transform.position = currentPosition;

            // 복귀 중에는 최대 한 번 추가 피해
            if (!hitDuringReturn)
            {
                hitDuringReturn = TryHitPlayerAt(currentPosition);
            }

            // 경고선의 끝점도 캐릭터를 따라 시작점으로 돌아간다.
            Vector2 currentLineEnd = Vector2.Lerp(
                fullLineEnd,
                attackStartPoint,
                t
            );

            warningLine.SetPosition(0, attackStartPoint);
            warningLine.SetPosition(1, currentLineEnd);

            timer += Time.deltaTime;
            yield return null;
        }

        // 캐릭터와 경고선의 마지막 위치를 정확히 고정
        movingAttacker.transform.position = characterStartPosition;

        warningLine.SetPosition(0, attackStartPoint);
        warningLine.SetPosition(1, attackStartPoint);
    }
    private Vector2 GetStartPointFromScreenEdge(Vector2 hitPoint)
    {
        if (CameraBounds2D.I == null)
        {
            return hitPoint + Vector2.left * 5f;
        }

        Rect rect = CameraBounds2D.I.GetWorldRect(0f);

        // 왼쪽과 오른쪽 중 하나를 무작위로 선택한다.
        bool fromLeft = Random.value < 0.5f;

        if (fromLeft)
        {
            return new Vector2(rect.xMin, hitPoint.y);
        }

        return new Vector2(rect.xMax, hitPoint.y);
    }
    private void SetAttackerDirection(Vector2 direction)
    {
        if (attackerRenderer == null)
        {
            return;
        }

        // 오른쪽으로 움직일 때는 원본 방향
        // 왼쪽으로 움직일 때는 이미지 반전
        attackerRenderer.flipX = direction.x < 0f;
    }



    private void HitPlayerIfInside()
    {
        Vector2 direction = attackEndPoint - attackStartPoint;
        float length = direction.magnitude;

        if (length <= 0.01f)
        {
            return;
        }

        Vector2 center = (attackStartPoint + attackEndPoint) * 0.5f;
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;

        Vector2 size = new Vector2(length, lineWidth);

        Collider2D hit = Physics2D.OverlapBox(center, size, angle, playerLayer);

        if (hit == null)
        {
            return;
        }

        BeeHealth health = hit.GetComponent<BeeHealth>();

        if (health != null)
        {
            health.TakeDamage(damage);
        }
    }
    private bool TryHitPlayerAt(Vector2 position)
    {
        Collider2D hit = Physics2D.OverlapCircle(
            position,
            attackerHitRadius,
            playerLayer
        );

        if (hit == null)
        {
            return false;
        }

        BeeHealth health = hit.GetComponent<BeeHealth>();

        if (health == null)
        {
            return false;
        }

        // Radioactive 오라 또는 피격 후 무적 상태라면
        // 피해와 카메라 흔들림을 모두 실행하지 않는다.
        if (health.IsInvincible)
        {
            return false;
        }

        health.TakeDamage(damage);

        // 실제로 피해가 적용된 경우에만 카메라 흔들림
        if (CameraShake.I != null)
        {
            CameraShake.I.Shake();
        }

        return true;
    }
    private void ClampEndPointToMinimumDistance()
    {
        if (CameraBounds2D.I == null)
        {
            return;
        }

        Rect rect = CameraBounds2D.I.GetWorldRect(0f);

        float minimumDistance =
            rect.width * minimumTravelRatio;

        bool startsFromLeft =
            attackStartPoint.x < rect.center.x;

        if (startsFromLeft)
        {
            float minimumEndX =
                rect.xMin + minimumDistance;

            attackEndPoint.x = Mathf.Max(
                attackEndPoint.x,
                minimumEndX
            );
        }
        else
        {
            float maximumEndX =
                rect.xMax - minimumDistance;

            attackEndPoint.x = Mathf.Min(
                attackEndPoint.x,
                maximumEndX
            );
        }

        // 루트 오브젝트 위치도 보정된 목표 지점에 맞춘다.
        transform.position = attackEndPoint;
    }

    private void OnDrawGizmos()
    {
        if (movingAttacker == null)
        {
            return;
        }

        Gizmos.color = new Color(1f, 1f, 0f, 0.8f);

        Gizmos.DrawWireSphere(
            movingAttacker.transform.position,
            attackerHitRadius
        );
    }
}