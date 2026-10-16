using System.Collections;
using UnityEngine;

public class BatAttackSpawner : MonoBehaviour
{
    [Header("Spawn")]
    [SerializeField] private GameObject batAttackPrefab;

    [SerializeField] private float initialInterval = 2f;
    [SerializeField] private float minimumInterval = 0.45f;
    [SerializeField] private float difficultyRampTime = 90f;
    [SerializeField] private float intervalRandomRange = 0.2f;

    [Header("Group Spawn")]
    [SerializeField] private int initialGroupSize = 2;
    [SerializeField] private int maximumGroupSize = 5;
    [SerializeField] private float groupSpawnDelay = 0.2f;
    [SerializeField] private float groupDelayRandomRange = 0.1f;

    [Header("Spawn Area")]
    [SerializeField] private float spawnMargin = 0.8f;
    [SerializeField] private float minimumSpawnDistance = 1.5f;
    [SerializeField] private int positionPickAttempts = 10;

    private float elapsedGameTime;
    private Vector2 lastSpawnPosition;
    private bool hasLastSpawnPosition;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        StartCoroutine(SpawnRoutine());
    }
    private IEnumerator SpawnRoutine()
    {
        while (GameFlow.I == null || !GameFlow.I.IsRunning)
        {
            yield return null;
        }

        elapsedGameTime = 0f;

        while (!GameFlow.I.IsGameOver)
        {
            float difficultyT = Mathf.Clamp01(
                elapsedGameTime / difficultyRampTime
            );

            float currentInterval = Mathf.Lerp(
                initialInterval,
                minimumInterval,
                difficultyT
            );

            float waitTime =
                currentInterval +
                Random.Range(
                    -intervalRandomRange,
                    intervalRandomRange
                );

            waitTime = Mathf.Max(
                minimumInterval,
                waitTime
            );

            yield return new WaitForSeconds(waitTime);

            int currentGroupSize = Mathf.RoundToInt(
    Mathf.Lerp(
        initialGroupSize,
        maximumGroupSize,
        difficultyT
    )
);

            yield return StartCoroutine(
                SpawnAttackGroup(currentGroupSize)
            );

            elapsedGameTime += waitTime;
        }
    }
    private IEnumerator SpawnAttackGroup(int groupSize)
    {
        for (int i = 0; i < groupSize; i++)
        {
            if (GameFlow.I == null || GameFlow.I.IsGameOver)
            {
                yield break;
            }

            SpawnBatAttack();

            // 마지막 사람 뒤에는 기다릴 필요가 없다.
            if (i < groupSize - 1)
            {
                float delay = groupSpawnDelay +
                    Random.Range(
                        -groupDelayRandomRange,
                        groupDelayRandomRange
                    );

                delay = Mathf.Max(0f, delay);

                yield return new WaitForSeconds(delay);
            }
        }
    }

    private void SpawnBatAttack()
    {
        if (batAttackPrefab == null)
        {
            Debug.LogWarning(
                "[BatAttackSpawner] BatAttack Prefab이 연결되지 않았습니다."
            );

            return;
        }

        Vector2 spawnPos = GetRandomPositionInScreen();

        Instantiate(
            batAttackPrefab,
            spawnPos,
            Quaternion.identity
        );
    }

    private Vector2 GetRandomPositionInScreen()
    {
        if (CameraBounds2D.I == null)
        {
            return Vector2.zero;
        }

        Rect rect = CameraBounds2D.I.GetWorldRect(0f);

        Vector2 selectedPosition = rect.center;

        for (int i = 0; i < positionPickAttempts; i++)
        {
            float x = Random.Range(
                rect.xMin + spawnMargin,
                rect.xMax - spawnMargin
            );

            float y = Random.Range(
                rect.yMin + spawnMargin,
                rect.yMax - spawnMargin
            );

            Vector2 candidate = new Vector2(x, y);

            if (!hasLastSpawnPosition)
            {
                selectedPosition = candidate;
                break;
            }

            float distanceFromLast =
                Vector2.Distance(candidate, lastSpawnPosition);

            if (distanceFromLast >= minimumSpawnDistance)
            {
                selectedPosition = candidate;
                break;
            }

            selectedPosition = candidate;
        }

        lastSpawnPosition = selectedPosition;
        hasLastSpawnPosition = true;

        return selectedPosition;
    }
    // Update is called once per frame

}
