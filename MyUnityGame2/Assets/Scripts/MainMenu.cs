using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenu : MonoBehaviour
{
    [Header("Buttons")]
    [SerializeField] private Button startButton;
    [SerializeField] private Button continueButton;
    [SerializeField] private Button quitButton;

    [Header("Scenes")]
    [SerializeField] private string firstGameScene = "Room1_Reception";

    void Awake()
    {
        startButton.onClick.AddListener(StartNewGame);
        continueButton.onClick.AddListener(ContinueGame);
        quitButton.onClick.AddListener(QuitGame);

        continueButton.interactable = SaveGame.HasSave();
    }

    void StartNewGame()
    {
        SaveGame.Clear(); 
        SaveGame.MarkHasSave(firstGameScene); 
        SceneManager.LoadScene(firstGameScene);
    }

    void ContinueGame()
    {
        if (SaveGame.TryGet(out string scene, out Vector3 _))
            SceneManager.LoadScene(scene);
        else
            SceneManager.LoadScene(firstGameScene); 
    }

    void QuitGame()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }
}