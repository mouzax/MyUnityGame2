using System.Collections;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class LightingController : MonoBehaviour
{
    [SerializeField] Light2D globalLight;    // assign GlobalLight
    [SerializeField] Light2D playerFlashlight; // assign Player/Flashlight
    [SerializeField] float fadeSeconds = 1.0f;

    bool powerOn = false;
    Coroutine fadeCo;

    void Start()
    {
        // start fully dark, flashlight on
        if (globalLight) globalLight.intensity = 0f;
        if (playerFlashlight) playerFlashlight.gameObject.SetActive(true);
    }

    public void TurnPowerOn()
    {
        if (powerOn) return; // one-time
        powerOn = true;
        if (fadeCo != null) StopCoroutine(fadeCo);
        fadeCo = StartCoroutine(FadeGlobal(1f));
        if (playerFlashlight) playerFlashlight.gameObject.SetActive(false);
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
