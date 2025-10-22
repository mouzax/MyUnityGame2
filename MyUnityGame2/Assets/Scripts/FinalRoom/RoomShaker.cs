using UnityEngine;

public class RoomShaker : MonoBehaviour
{
    [Header("Target to Shake (e.g., the whole room root)")]
    [SerializeField] Transform target;

    [Header("Shake Settings")]
    [SerializeField] float amplitude = 0.05f;
    [SerializeField] float frequency = 18f;

    Vector3 _basePos;
    bool _isShaking = true;

    void Awake()
    {
        if (target == null) target = transform;
        _basePos = target.localPosition;
    }

    void Update()
    {
        if (!_isShaking) return;

        float t = Time.time * frequency;
        Vector3 offset = new Vector3(
            (Mathf.PerlinNoise(t, 0f) - 0.5f),
            (Mathf.PerlinNoise(0f, t) - 0.5f),
            0f
        ) * amplitude;

        target.localPosition = _basePos + offset;
    }

    public void StartShake()
    {
        _isShaking = true;
    }

    public void StopShake()
    {
        _isShaking = false;
        if (target != null) target.localPosition = _basePos;
    }
}