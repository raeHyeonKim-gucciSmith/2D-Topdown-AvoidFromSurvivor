using UnityEngine;

public class DestroyAfterAnimation : MonoBehaviour
{
    [SerializeField] private float lifetime = 0.85f;

    private void Start()
    {
        Destroy(gameObject, lifetime);
    }
}