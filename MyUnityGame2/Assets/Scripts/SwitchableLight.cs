using UnityEngine;
using UnityEngine.Events;
using System.Collections;
#if USING_URP
using UnityEngine.Rendering.Universal;
#endif

public class SwitchableLight : MonoBehaviour
{
    [Header("Refs")]
    [SerializeField] MonoBehaviour lightRef; // SpriteLight2D or any component with 'enabled' + intensity
    [SerializeField] float onIntensity = 1.5f;
    [SerializeField] float offIntensity = 0f;
    [SerializeField] float fadeTime = 0.25f;
    [SerializeField] ObjectBacklight2D backlight; // optional: highlight ring/glow

    [Header("State")]
    [SerializeField] bool startsOn = true;
    public bool IsOn { get; private set; }

    public UnityEvent OnTurnedOff = new UnityEvent();
    public UnityEvent OnTurnedOn  = new UnityEvent();

    float GetIntensity()
    {
        #if USING_URP
        var l = lightRef as Light2D;
        return l != null ? l.intensity : 0f;
        #else
        // fallback in case you're abstracting via LightingController
        return 0f;
        #endif
    }

    void SetIntensity(float v)
    {
        #if USING_URP
        var l = lightRef as Light2D;
        if (l != null) l.intensity = v;
        #endif
    }

    void Awake()
    {
        IsOn = startsOn;
        SetIntensity(IsOn ? onIntensity : offIntensity);
        if (backlight) backlight.SetActive(IsOn); // only show glow while on
    }

    public void ToggleOrTurnOff()
    {
        if (!IsOn) return;
        StopAllCoroutines();
        StartCoroutine(FadeToOff());
    }

    public void ForceOn()
    {
        StopAllCoroutines();
        StartCoroutine(FadeTo(onIntensity, true));
    }

    public void ForceOff()
    {
        StopAllCoroutines();
        StartCoroutine(FadeTo(offIntensity, false));
    }

    IEnumerator FadeToOff() => FadeTo(offIntensity, false);

    IEnumerator FadeTo(float target, bool onAfter)
    {
        float start = GetIntensity();
        float t = 0f;
        while (t < fadeTime)
        {
            t += Time.deltaTime;
            SetIntensity(Mathf.Lerp(start, target, t / fadeTime));
            yield return null;
        }
        SetIntensity(target);
        IsOn = onAfter;
        if (backlight) backlight.SetActive(IsOn);

        if (IsOn) OnTurnedOn.Invoke();
        else OnTurnedOff.Invoke();
    }
}