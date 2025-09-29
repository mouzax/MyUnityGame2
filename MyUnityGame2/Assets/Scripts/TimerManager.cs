using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;


public class TimerManager : MonoBehaviour
{
    [Header("Day Settings")]
    [SerializeField] float dayLengthMinutes = 15f;
    [SerializeField] int totalDays = 7;

    [Header("UI (optional)")]
    [SerializeField] TextMeshProUGUI day;
    [SerializeField] TextMeshProUGUI time;

    private float currentDayTime = 0f;
    private int currentDay = 1;
    private bool isRunning = true;

    public static TimerManager Instance;

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }
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
        if (day != null)
            day.text = $"Day {currentDay}/{totalDays}";

        if (time != null)
        {
            float remaining = dayLengthMinutes * 60f - currentDayTime;
            if (remaining < 0) remaining = 0;
            int minutes = Mathf.FloorToInt(remaining / 60f);
            int seconds = Mathf.FloorToInt(remaining % 60f);
            time.text = $"{minutes:00}:{seconds:00}";
        }
    }

    void EndGame()
    {
        isRunning = false;
        Debug.Log("Game Over – 7 days are finished!");
    }

    void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (day == null)
        {
            var dayObj = GameObject.FindWithTag("Day");
            if (dayObj) day = dayObj.GetComponent<TextMeshProUGUI>();
        }
        if (time == null)
        {
            var timerObj = GameObject.FindWithTag("Time");
            if (timerObj) time = timerObj.GetComponent<TextMeshProUGUI>();
        }

        UpdateUI();
    }

}