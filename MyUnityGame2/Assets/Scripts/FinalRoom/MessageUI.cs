using UnityEngine;
using TMPro;
using System.Collections;

public class MessageUI : MonoBehaviour
{
    [Header("Assign TMP text objects")]
    [SerializeField] TextMeshProUGUI promptText;
    [SerializeField] TextMeshProUGUI feedbackText; 

    [Header("Timings")]
    [SerializeField] float feedbackDuration = 1.2f;

    Coroutine _feedbackRoutine;

    void Awake()
    {
        if (promptText) promptText.gameObject.SetActive(false);
        if (feedbackText) feedbackText.gameObject.SetActive(false);
    }

    public void ShowPrompt(bool show)
    {
        if (!promptText) return;
        promptText.gameObject.SetActive(show);
        if (show) promptText.text = "Press Space to Inspect";
    }

    public void ShowCollected(string msg)
    {
        if (!feedbackText) return;

        if (_feedbackRoutine != null) StopCoroutine(_feedbackRoutine);
        _feedbackRoutine = StartCoroutine(ShowFeedbackRoutine(msg));
    }

    IEnumerator ShowFeedbackRoutine(string msg)
    {
        feedbackText.text = msg;
        feedbackText.gameObject.SetActive(true);
        yield return new WaitForSeconds(feedbackDuration);
        feedbackText.gameObject.SetActive(false);
        _feedbackRoutine = null;
    }
}