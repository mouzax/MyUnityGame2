using UnityEngine;
using TMPro;
using System.Collections;

public class SceneMessage : MonoBehaviour
{
    public static SceneMessage Instance;

    [SerializeField] TextMeshProUGUI messageText;
    [SerializeField] float fadeDuration = 0.4f;
    [SerializeField] float visibleDuration = 1.5f;

    Coroutine currentMessage;

    void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(gameObject); return; }
        Instance = this;

        if (messageText)
            messageText.gameObject.SetActive(false);
    }

    public void Show(string message)
    {
        if (!messageText) return;

        if (currentMessage != null)
            StopCoroutine(currentMessage);

        messageText.text = message;
        messageText.alpha = 1f;
        messageText.gameObject.SetActive(true);
    }

    public void ShowTemporary(string message)
    {
        if (currentMessage != null)
            StopCoroutine(currentMessage);

        currentMessage = StartCoroutine(ShowThenFade(message));
    }

    IEnumerator ShowThenFade(string message)
    {
        Show(message);

        yield return new WaitForSecondsRealtime(visibleDuration);

        float elapsed = 0f;
        Color c = messageText.color;
        while (elapsed < fadeDuration)
        {
            elapsed += Time.unscaledDeltaTime;
            c.a = Mathf.Lerp(1f, 0f, elapsed / fadeDuration);
            messageText.color = c;
            yield return null;
        }

        messageText.gameObject.SetActive(false);
        c.a = 1f;
        messageText.color = c;
        currentMessage = null;
    }

    public void Hide()
    {
        if (currentMessage != null)
            StopCoroutine(currentMessage);
        if (messageText)
        {
            messageText.gameObject.SetActive(false);
            var c = messageText.color;
            c.a = 1f;
            messageText.color = c;
        }
    }
}