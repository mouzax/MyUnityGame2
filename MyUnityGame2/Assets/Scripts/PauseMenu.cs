using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine.EventSystems;

public class PauseMenu : MonoBehaviour
{
    [Header("UI refs")]
    [SerializeField] private GameObject pausePanel;
    [SerializeField] private Button resumeButton;
    [SerializeField] private Button menuButton;
    [SerializeField] private Button restartButton;
    [SerializeField] private Button quitButton;

    [Header("Scenes")]
    [SerializeField] private string mainMenuScene = "Menu";
    [SerializeField] private string firstGameScene = "Room1_Reception";

    [Header("Input")]
    [SerializeField] private KeyCode toggleKey = KeyCode.Escape;

    private void Awake()
    {
        // Wire buttons
        if (resumeButton  != null) resumeButton.onClick.AddListener(Resume);
        if (menuButton    != null) menuButton.onClick.AddListener(GoToMainMenu);
        if (restartButton != null) restartButton.onClick.AddListener(RestartFromBeginning);
        if (quitButton    != null) quitButton.onClick.AddListener(QuitGame);

        Time.timeScale = 1f;
        HidePanel();
    }

    private void Update()
    {
        if (Input.GetKeyDown(toggleKey))
            TogglePause();
    }

    public void TogglePause()
    {
        if (IsPaused()) Resume();
        else Pause();
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

    public void GoToMainMenu()
    {
        Time.timeScale = 1f;
        AudioListener.pause = false;
        SceneManager.LoadScene(mainMenuScene);
    }

    public void RestartFromBeginning()
    {
        Time.timeScale = 1f;
        AudioListener.pause = false;


        SceneManager.LoadScene(firstGameScene);
    }

    public void QuitGame()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }

    private bool IsPaused() { return Time.timeScale == 0f; }
    private void ShowPanel() { if (pausePanel) pausePanel.SetActive(true); }
    private void HidePanel() { if (pausePanel) pausePanel.SetActive(false); }
}