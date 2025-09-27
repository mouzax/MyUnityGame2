using UnityEngine;
using System.Linq;
using UnityEngine.SceneManagement;

public class SwitchManager : MonoBehaviour
{
    [Header("Scene Transition")]
    [SerializeField] string nextSceneName = "Main";
    [SerializeField] int    nextSceneIndex = -1;
    [SerializeField] float  loadDelay = 0.7f;

    [Header("UI (optional)")]
    [SerializeField] UIMessage messageUI;
    [SerializeField] string  allOnMessage = "All switches activated! Moving to final room...";

    private LabSwitch[] allSwitches;
    private bool isLoading;

    void Start()
    {
        RefreshSwitchList();
        LabSwitch.OnActivated += HandleSwitchActivated;

        if (messageUI == null) messageUI = FindObjectOfType<UIMessage>();
    }

    void OnDestroy()
    {
        LabSwitch.OnActivated -= HandleSwitchActivated;
    }

    void HandleSwitchActivated(LabSwitch s)
    {
        if (!isLoading && AllOn())
        {
            if (messageUI != null && !string.IsNullOrEmpty(allOnMessage))
                messageUI.Show(allOnMessage, loadDelay);

            isLoading = true;
            Invoke(nameof(LoadNextScene), loadDelay);
        }
    }

    public bool AllOn()
    {
        if (allSwitches == null || allSwitches.Length == 0)
            RefreshSwitchList();

        return allSwitches != null && allSwitches.All(sw => sw.IsOn);
    }

    public static event System.Action OnSwitchesReset;
    public void ResetAll()
    {
        if (allSwitches == null || allSwitches.Length == 0)
            RefreshSwitchList();

        foreach (var sw in allSwitches) sw.ResetSwitch();
        isLoading = false;
        OnSwitchesReset?.Invoke();
    }

    void RefreshSwitchList()
    {
        allSwitches = FindObjectsOfType<LabSwitch>(includeInactive: false);
    }

    void LoadNextScene()
    {
        if (nextSceneIndex >= 0)
        {
            SceneManager.LoadScene(nextSceneIndex);
        }
        else if (!string.IsNullOrEmpty(nextSceneName))
        {
            SceneManager.LoadScene(nextSceneName);
        }
        else
        {
            Debug.LogError("[SwitchManager] No next scene set. Provide nextSceneName or nextSceneIndex.");
        }
    }
}