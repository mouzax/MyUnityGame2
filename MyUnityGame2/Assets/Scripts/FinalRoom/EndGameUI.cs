using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class EndGameUI : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] GameObject winPanel;
    [SerializeField] TextMeshProUGUI titleText;
    [SerializeField] TextMeshProUGUI bodyText;
    [SerializeField] Button restartButton;
    [SerializeField] Button quitButton;

    bool shown = false;

    void Awake()
    {
        if (winPanel) winPanel.SetActive(false);
        if (restartButton) restartButton.onClick.AddListener(RestartGame);
        if (quitButton) quitButton.onClick.AddListener(QuitGame);
    }

    public void ShowWin(string title = "You Escaped!", string body = "Power restored. Doors unlocked. You made it!")
    {
        if (shown) return;
        shown = true;

        if (titleText) titleText.text = title;
        if (bodyText) bodyText.text = body;
        if (winPanel) winPanel.SetActive(true);

        Time.timeScale = 0f;
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    void RestartGame()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(0);
    }

    void QuitGame()
    {
        Time.timeScale = 1f;
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }
}