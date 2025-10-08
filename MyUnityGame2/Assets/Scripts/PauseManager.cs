using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine.EventSystems;

public class PauseManager : MonoBehaviour
{
    public static PauseManager Instance;

    [Header("UI")]
    [SerializeField] private GameObject pausePanel;
    [SerializeField] private Button resumeButton;
    [SerializeField] private Button resetButton;
    [SerializeField] private Button quitButton;

    [Header("Input")]
    [SerializeField] private KeyCode toggleKey = KeyCode.Escape;

    void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(gameObject); return; }
        Instance = this;
        DontDestroyOnLoad(gameObject);

        if (resumeButton != null) resumeButton.onClick.AddListener(Resume);
        if (resetButton  != null) resetButton.onClick.AddListener(ResetLevel);
        if (quitButton   != null) quitButton.onClick.AddListener(QuitGame);

        HidePanel();
        Time.timeScale = 1f;

        SceneManager.activeSceneChanged += OnSceneChanged;
    }

    void OnDestroy()
    {
        if (Instance == this) SceneManager.activeSceneChanged -= OnSceneChanged;
    }

    void Update()
    {
        if (Input.GetKeyDown(toggleKey)) TogglePause();
    }

    void OnSceneChanged(Scene oldScene, Scene newScene)
    {
        Time.timeScale = 1f;
        HidePanel();
    }

    public static void Toggle() { if (Instance != null) Instance.TogglePause(); }

    public void TogglePause()
    {
        if (IsPaused()) Resume(); else Pause();
    }

    public void Pause()
    {
        ShowPanel();
        Time.timeScale = 0f;
        AudioListener.pause = true;

        if (resumeButton != null && EventSystem.current != null)
            EventSystem.current.SetSelectedGameObject(resumeButton.gameObject);
    }

    public void Resume()
    {
        HidePanel();
        Time.timeScale = 1f;
        AudioListener.pause = false;
    }

    public void ResetLevel()
    {
        Time.timeScale = 1f;
        AudioListener.pause = false;
        Scene active = SceneManager.GetActiveScene();
        SceneManager.LoadScene(active.name);
    }

    public void QuitGame()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }

    private bool IsPaused()
    {
        return Time.timeScale == 0f;
    }

    private void ShowPanel()
    {
        if (pausePanel != null) pausePanel.SetActive(true);
    }

    private void HidePanel()
    {
        if (pausePanel != null) pausePanel.SetActive(false);
    }
}