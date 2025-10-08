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

<<<<<<< Updated upstream
    [Header("Find Player")]
    [SerializeField] private string playerTag = "Player";

    private void Awake()
    {
        if (resumeButton)  resumeButton.onClick.AddListener(Resume);
        if (menuButton)    menuButton.onClick.AddListener(GoToMainMenu);
        if (restartButton) restartButton.onClick.AddListener(RestartFromBeginning);
        if (quitButton)    quitButton.onClick.AddListener(QuitGame);
=======
    private void Awake()
    {
        // Wire buttons
        if (resumeButton  != null) resumeButton.onClick.AddListener(Resume);
        if (menuButton    != null) menuButton.onClick.AddListener(GoToMainMenu);
        if (restartButton != null) restartButton.onClick.AddListener(RestartFromBeginning);
        if (quitButton    != null) quitButton.onClick.AddListener(QuitGame);
>>>>>>> Stashed changes

        Time.timeScale = 1f;
        HidePanel();
    }

    private void Update()
    {
<<<<<<< Updated upstream
        if (Input.GetKeyDown(toggleKey)) TogglePause();
    }

    public void TogglePause() { if (IsPaused()) Resume(); else Pause(); }
=======
        if (Input.GetKeyDown(toggleKey))
            TogglePause();
    }

    public void TogglePause()
    {
        if (IsPaused()) Resume();
        else Pause();
    }

>>>>>>> Stashed changes
    public void Pause()
    {
        ShowPanel();
        Time.timeScale = 0f;
        AudioListener.pause = true;
<<<<<<< Updated upstream
        if (resumeButton && EventSystem.current)
            EventSystem.current.SetSelectedGameObject(resumeButton.gameObject);
    }
=======

        if (resumeButton != null && EventSystem.current != null)
            EventSystem.current.SetSelectedGameObject(resumeButton.gameObject);
    }

>>>>>>> Stashed changes
    public void Resume()
    {
        HidePanel();
        Time.timeScale = 1f;
        AudioListener.pause = false;
    }

    public void GoToMainMenu()
    {
<<<<<<< Updated upstream
        SaveCurrentSpot();

=======
>>>>>>> Stashed changes
        Time.timeScale = 1f;
        AudioListener.pause = false;
        SceneManager.LoadScene(mainMenuScene);
    }

    public void RestartFromBeginning()
    {
<<<<<<< Updated upstream

       Time.timeScale = 1f;
       AudioListener.pause = false;
       TimerManager.RequestHardResetOnNextLoad();
       SceneManager.LoadScene(firstGameScene);
=======
        Time.timeScale = 1f;
        AudioListener.pause = false;


        SceneManager.LoadScene(firstGameScene);
>>>>>>> Stashed changes
    }

    public void QuitGame()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }

<<<<<<< Updated upstream
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

=======
>>>>>>> Stashed changes
    private bool IsPaused() { return Time.timeScale == 0f; }
    private void ShowPanel() { if (pausePanel) pausePanel.SetActive(true); }
    private void HidePanel() { if (pausePanel) pausePanel.SetActive(false); }
}