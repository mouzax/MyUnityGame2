using UnityEngine;
using TMPro;

public class TimerManager : MonoBehaviour
{
    [Header("Day Settings")]
    [SerializeField] float dayLengthMinutes = 15f;
    [SerializeField] int totalDays = 7;

    [Header("UI (optional)")]
    [SerializeField] TextMeshProUGUI dayText;
    [SerializeField] TextMeshProUGUI timerText;

    private float currentDayTime = 0f;
    private int currentDay = 1;
    private bool isRunning = true;

    void Update()
    {
        if (!isRunning) return;

        currentDayTime += Time.deltaTime;

        if (currentDayTime >= dayLengthMinutes * 60f)
        {
            currentDay++;
            currentDayTime = 0f;

            if (currentDay > totalDays)
            {
                EndGame();
                return;
            }
        }

        UpdateUI();
    }

    void UpdateUI()
    {
        if (dayText != null)
            dayText.text = $"Day {currentDay}/{totalDays}";

        if (timerText != null)
        {
            float remaining = dayLengthMinutes * 60f - currentDayTime;
            if (remaining < 0) remaining = 0;
            int minutes = Mathf.FloorToInt(remaining / 60f);
            int seconds = Mathf.FloorToInt(remaining % 60f);
            timerText.text = $"{minutes:00}:{seconds:00}";
        }
    }

    void EndGame()
    {
        isRunning = false;
        Debug.Log("Game Over – 7 days are finished!");
    }
}