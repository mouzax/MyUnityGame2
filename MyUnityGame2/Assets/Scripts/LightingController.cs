using System.Collections;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class LightingController : MonoBehaviour
{
    [SerializeField] Light2D globalLight;
    [SerializeField] Light2D playerFlashlight;
    [SerializeField] float fadeSeconds = 1.0f;
    [SerializeField] Mission1State mission1;

    // Audio
    [SerializeField] AudioSource ambientSource;
    [SerializeField] AudioSource sfxSource;
    [SerializeField] AudioClip switchFlipSFX;
    [SerializeField] AudioClip flickerSFX;

    bool powerOn = false;
    Coroutine fadeCo;

    void Start()
    {
        if (globalLight) globalLight.intensity = 0f;
        if (playerFlashlight) playerFlashlight.gameObject.SetActive(true);

        // Audio
        if (ambientSource != null)
            ambientSource.Play();
    }

    public void TurnPowerOn()
    {
        if (powerOn) return;
        powerOn = true;
        if (fadeCo != null) StopCoroutine(fadeCo);
        StartCoroutine(PowerOnSequence()); //Audio
        fadeCo = StartCoroutine(FadeGlobal(1f));
        if (playerFlashlight) playerFlashlight.gameObject.SetActive(false);
        if (mission1) mission1.SetPowerOn();
    }

    //Audio
    private IEnumerator PowerOnSequence()
    {
        if (ambientSource != null && ambientSource.isPlaying)
            ambientSource.Stop();

        if (sfxSource != null && switchFlipSFX != null)
            sfxSource.PlayOneShot(switchFlipSFX);

        yield return new WaitForSeconds(0.3f);

        if (sfxSource != null && flickerSFX != null)
            sfxSource.PlayOneShot(flickerSFX);

        for (int i = 0; i < 3; i++)
        {
            if (globalLight) globalLight.intensity = 0.3f;
            yield return new WaitForSeconds(0.1f);
            if (globalLight) globalLight.intensity = 0f;
            yield return new WaitForSeconds(0.1f);
        }

        fadeCo = StartCoroutine(FadeGlobal(1f));
        Debug.Log("PowerOnSequence started!");
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
