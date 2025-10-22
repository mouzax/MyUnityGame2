using UnityEngine;

public class CameraShake : MonoBehaviour
{
    public static CameraShake Instance;

    [Header("Default Settings")]
    [SerializeField] float defaultDuration = 0.5f;
    [SerializeField] float defaultStrength = 0.4f;
    [SerializeField] float defaultRotStrength = 8f;
    [SerializeField] AnimationCurve falloff = AnimationCurve.EaseInOut(0, 1, 1, 0);

    private Vector3 originalPos;
    private Quaternion originalRot;

    private float shakeTimer;
    private float shakeDuration;
    private float posStrength;
    private float rotStrength;
    private bool isShaking;
    private bool stopRequested;

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        originalPos = transform.localPosition;
        originalRot = transform.localRotation;
    }

    void LateUpdate()
    {
        if (!isShaking) return;

        shakeTimer += Time.unscaledDeltaTime;
        float t = Mathf.Clamp01(shakeTimer / shakeDuration);
        float fall = falloff.Evaluate(t);

        float px = (Mathf.PerlinNoise(Time.time * 15f, 0f) - 0.5f) * 2f;
        float py = (Mathf.PerlinNoise(0f, Time.time * 15f) - 0.5f) * 2f;
        float rz = (Mathf.PerlinNoise(Time.time * 20f, 5f) - 0.5f) * 2f;

        transform.localPosition = originalPos + new Vector3(px, py, 0) * posStrength * fall;
        transform.localRotation = Quaternion.Euler(0, 0, rz * rotStrength * fall);

        if (stopRequested || shakeTimer >= shakeDuration)
        {
            StopShakeImmediate();
        }
    }

    public void Shake(float duration = -1f, float strength = -1f, float rot = -1f)
    {
        isShaking = true;
        stopRequested = false;

        shakeDuration = duration > 0 ? duration : defaultDuration;
        posStrength = strength > 0 ? strength : defaultStrength;
        rotStrength = rot > 0 ? rot : defaultRotStrength;

        shakeTimer = 0f;
        originalPos = transform.localPosition;
        originalRot = transform.localRotation;
    }

    public void StopShakeImmediate()
    {
        isShaking = false;
        stopRequested = false;
        transform.localPosition = originalPos;
        transform.localRotation = originalRot;
    }

    public void StopShake(float fadeTime)
    {
        StartCoroutine(FadeOut(fadeTime));
    }

    private System.Collections.IEnumerator FadeOut(float fadeTime)
    {
        float startStrength = posStrength;
        float startRot = rotStrength;
        float t = 0;

        while (t < fadeTime)
        {
            t += Time.deltaTime;
            float lerp = 1f - (t / fadeTime);
            posStrength = startStrength * lerp;
            rotStrength = startRot * lerp;
            yield return null;
        }

        StopShakeImmediate();
    }
}