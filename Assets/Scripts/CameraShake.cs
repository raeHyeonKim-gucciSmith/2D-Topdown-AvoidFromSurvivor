using System.Collections;
using UnityEngine;

public class CameraShake : MonoBehaviour
{
    public static CameraShake I { get; private set; }

    [Header("Shake Settings")]
    [SerializeField] private float defaultDuration = 0.15f;
    [SerializeField] private float defaultStrength = 0.15f;

    private Vector3 originalPosition;
    private Coroutine shakeCoroutine;

    private void Awake()
    {
        I = this;
        originalPosition = transform.position;
    }

    public void Shake()
    {
        Shake(defaultDuration, defaultStrength);
    }
    public void Shake(float duration, float strength)
    {
        if (shakeCoroutine != null)
        {
            StopCoroutine(shakeCoroutine);
            transform.position = originalPosition;
        }

        shakeCoroutine = StartCoroutine(ShakeRoutine(duration, strength));
    }
    private IEnumerator ShakeRoutine(float duration, float strength)
    {
        float timer = 0f;

        while (timer < duration)
        {
            float x = Random.Range(-strength, strength);
            float y = Random.Range(-strength, strength);

            transform.position = originalPosition + new Vector3(x, y, 0f);

            timer += Time.deltaTime;
            yield return null;
        }

        transform.position = originalPosition;
        shakeCoroutine = null;
    }
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }
}
