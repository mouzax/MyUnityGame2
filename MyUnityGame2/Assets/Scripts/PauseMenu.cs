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

    [Header("Find Player")]
    [SerializeField] private string playerTag = "Player";

    private void Awake()
    {
        if (resumeButton)  resumeButton.onClick.AddListener(Resume);
        if (menuButton)    menuButton.onClick.AddListener(GoToMainMenu);
        if (restartButton) restartButton.onClick.AddListener(RestartFromBeginning);
        if (quitButton)    quitButton.onClick.AddListener(QuitGame);

        Time.timeScale = 1f;
        HidePanel();
    }

    private void Update()
    {
        if (Input.GetKeyDown(toggleKey)) TogglePause();
    }

    public void TogglePause() { if (IsPaused()) Resume(); else Pause(); }
    public void Pause()
    {
        ShowPanel();
        Time.timeScale = 0f;
        AudioListener.pause = true;
        if (resumeButton && EventSystem.current)
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
        SaveCurrentSpot();

        Time.timeScale = 1f;
        AudioListener.pause = false;
        SceneManager.LoadScene(mainMenuScene);
    }

    public void RestartFromBeginning()
    {

       Time.timeScale = 1f;
       AudioListener.pause = false;
       TimerManager.RequestHardResetOnNextLoad();
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

    private void SaveCurrentSpot()
    {
        GameObject playerObj = GameObject.FindGameObjectWithTag(playerTag);
        string scene = SceneManager.GetActiveScene().name;

        if (playerObj != null)
        {
            Vector3 pos = playerObj.transform.position;
            SaveGame.Save(scene, pos);
        }
        else
        {
            SaveGame.MarkHasSave(scene);
        }
    }

    private bool IsPaused() { return Time.timeScale == 0f; }
    private void ShowPanel() { if (pausePanel) pausePanel.SetActive(true); }
    private void HidePanel() { if (pausePanel) pausePanel.SetActive(false); }
}