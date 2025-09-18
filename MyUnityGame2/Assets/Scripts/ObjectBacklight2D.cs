using UnityEngine;

public class ObjectBacklight2D : MonoBehaviour
{
    [Header("Auto-Assigned Backlight (child)")]
    [SerializeField] private Light builtinLight;
    [SerializeField] private SpriteRenderer glowSprite;

    [Header("Behavior")]
    [SerializeField] private string childName = "Backlight";
    [SerializeField] private bool startOn = false;
    [SerializeField] private float onIntensity = 2.0f;
    [SerializeField] private float offIntensity = 0.0f;
    [SerializeField] private float fadeDuration = 0.18f;

    [Header("Colors")]
    [SerializeField] private Color onColor = Color.white;
    [SerializeField] private Color offColor = Color.white;

    private Coroutine fadeCo;

    void Awake()
    {
        if (builtinLight == null && glowSprite == null)
        {
            var child = FindChildRecursive(transform, childName);
            if (child == null) child = transform;

            builtinLight = child.GetComponentInChildren<Light>(true);
            glowSprite = child.GetComponentInChildren<SpriteRenderer>(true);
        }

        SetOn(startOn, true);
    }

    public void SetOn(bool isOn, bool instant = false)
    {
        if (fadeCo != null) { StopCoroutine(fadeCo); fadeCo = null; }

        if (instant || fadeDuration <= 0.0001f)
        {
            ApplyStateInstant(isOn);
        }
        else
        {
            fadeCo = StartCoroutine(FadeTo(isOn));
        }
    }

    private void ApplyStateInstant(bool isOn)
    {
        if (builtinLight != null)
        {
            builtinLight.intensity = isOn ? onIntensity : offIntensity;
            builtinLight.color = isOn ? onColor : offColor;
            builtinLight.enabled = (builtinLight.intensity > 0f);
        }

        if (glowSprite != null)
        {
            Color target = isOn ? onColor : offColor;
            glowSprite.color = target;
            glowSprite.enabled = target.a > 0.001f;
        }
    }

    private System.Collections.IEnumerator FadeTo(bool turnOn)
    {
        float blStartIntensity = builtinLight != null ? builtinLight.intensity : 0f;
        Color blStartColor = builtinLight != null ? builtinLight.color : Color.white;

        Color srStartColor = glowSprite != null ? glowSprite.color : Color.white;

        float targetIntensity = turnOn ? onIntensity : offIntensity;
        Color targetColor = turnOn ? onColor : offColor;

        if (builtinLight != null) builtinLight.enabled = true;
        if (glowSprite != null) glowSprite.enabled = true;

        float t = 0f;
        float dur = Mathf.Max(0.0001f, fadeDuration);

        while (t < 1f)
        {
            t += Time.deltaTime / dur;
            float k = Mathf.Clamp01(t);

            if (builtinLight != null)
            {
                builtinLight.intensity = Mathf.Lerp(blStartIntensity, targetIntensity, k);
                builtinLight.color = Color.Lerp(blStartColor, targetColor, k);
            }

            if (glowSprite != null)
            {
                glowSprite.color = Color.Lerp(srStartColor, targetColor, k);
            }

            yield return null;
        }

        if (builtinLight != null)
        {
            builtinLight.intensity = targetIntensity;
            builtinLight.color = targetColor;
            builtinLight.enabled = targetIntensity > 0f;
        }

        if (glowSprite != null)
        {
            glowSprite.color = targetColor;
            glowSprite.enabled = targetColor.a > 0.001f;
        }

        fadeCo = null;
    }

    private static Transform FindChildRecursive(Transform parent, string name)
    {
        if (parent.name == name) return parent;
        for (int i = 0; i < parent.childCount; i++)
        {
            var c = parent.GetChild(i);
            if (c.name == name) return c;
            var deep = FindChildRecursive(c, name);
            if (deep != null) return deep;
        }
        return null;
    }

    public void SetActive(bool on)
    {
        if (!gameObject) return;
        gameObject.SetActive(on);
    }

}