using System.Collections;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class LightingController : MonoBehaviour
{
    [SerializeField] Light2D globalLight;
    [SerializeField] Light2D playerFlashlight;
    [SerializeField] float fadeSeconds = 1.0f;
    [SerializeField] Mission1State mission1;

    bool powerOn = false;
    Coroutine fadeCo;

    void Start()
    {
        if (globalLight) globalLight.intensity = 0f;
        if (playerFlashlight) playerFlashlight.gameObject.SetActive(true);
    }

    public void TurnPowerOn()
    {
        if (powerOn) return;
        powerOn = true;
        if (fadeCo != null) StopCoroutine(fadeCo);
        fadeCo = StartCoroutine(FadeGlobal(1f));
        if (playerFlashlight) playerFlashlight.gameObject.SetActive(false);
        if (mission1) mission1.SetPowerOn();
    }

    IEnumerator FadeGlobal(float target)
    {
        if (!globalLight) yield break;
        float start = globalLight.intensity;
        float t = 0f;
        while (t < fadeSeconds)
        {
            t += Time.deltaTime;
            globalLight.intensity = Mathf.Lerp(start, target, t / fadeSeconds);
            yield return null;
        }
        globalLight.intensity = target;
    }
}
