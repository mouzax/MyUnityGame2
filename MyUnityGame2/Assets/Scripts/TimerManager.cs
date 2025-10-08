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

    public static TimerManager Instance;

    // internal state
    float currentDayTime = 0f;
    int currentDay = 1;
    bool isRunning = true;

    static bool _pendingHardReset = false;

    void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(gameObject); return; }
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    void OnDisable()
    {
        if (Instance == this)
            SceneManager.sceneLoaded -= OnSceneLoaded;
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

    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        // Rebind UI by tags if not assigned
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

        if (_pendingHardReset)
        {
            HardReset();
            _pendingHardReset = false;
        }

        UpdateUI();
    }

    void UpdateUI()
    {
        if (day) day.text = "Day " + currentDay + "/" + totalDays;

        if (time)
        {
            float remaining = dayLengthMinutes * 60f - currentDayTime;
            if (remaining < 0f) remaining = 0f;
            int minutes = Mathf.FloorToInt(remaining / 60f);
            int seconds = Mathf.FloorToInt(remaining % 60f);
            time.text = minutes.ToString("00") + ":" + seconds.ToString("00");
        }
    }

    void EndGame()
    {
        isRunning = false;
        Debug.Log("Game Over – " + totalDays + " days are finished!");
    }

    public void HardReset()
    {
        currentDayTime = 0f;
        currentDay = 1;
        isRunning = true;
        UpdateUI();
    }

    public static void RequestHardResetOnNextLoad()
    {
        _pendingHardReset = true; 
        if (Instance != null) Instance.HardReset();
    }

    public void PauseTimer(bool pause) { isRunning = !pause; }
}