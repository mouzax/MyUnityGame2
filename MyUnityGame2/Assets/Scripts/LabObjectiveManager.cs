using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class LabObjectiveManager : MonoBehaviour
{
    [Header("Targets & Hooks")]
    [SerializeField] MonsterController monster;
    [SerializeField] Vector2 spawnPosition = new Vector2(-9, 4);
    [SerializeField] UIMessage messageUI;
    [SerializeField] TMP_Text progressText;

    [Header("Next Scene")]
    [SerializeField] string nextSceneName = "Main";
    [SerializeField] float delayBeforeLoad = 1.2f;

    LabInteractable[] interactables;
    int total;
    int activatedCount;

    void OnEnable()  { LabInteractable.OnActivatedGlobal += HandleActivated; }
    void OnDisable() { LabInteractable.OnActivatedGlobal -= HandleActivated; }

    void Start()
    {
#if UNITY_2023_1_OR_NEWER
        var list = Object.FindObjectsByType<LabInteractable>(FindObjectsSortMode.None);
        interactables = list;
#else
        interactables = FindObjectsOfType<LabInteractable>(includeInactive:false);
#endif

        total = 0; activatedCount = 0;

        foreach (var li in interactables)
        {
            if (li.Type == LabThing.Microscope ||
                li.Type == LabThing.Puddle ||
                li.Type == LabThing.Paper ||
                li.Type == LabThing.Flask ||
                li.Type == LabThing.Book)
            {
                total++;
                if (li.Activated) activatedCount++;
            }
        }

        UpdateProgressUI();

        if (monster != null)
            monster.Spawn(spawnPosition);
    }

    void HandleActivated(LabInteractable li)
    {
        if (li.Type != LabThing.Microscope &&
            li.Type != LabThing.Puddle &&
            li.Type != LabThing.Paper &&
            li.Type != LabThing.Flask &&
            li.Type != LabThing.Book) return;

        activatedCount = Mathf.Clamp(activatedCount + 1, 0, total);
        UpdateProgressUI();

        if (activatedCount >= total)
            CompleteObjective();
    }

    void UpdateProgressUI()
    {
        if (progressText) progressText.text = $"Systems Restored: {activatedCount}/{total}";
    }

    void CompleteObjective()
    {
        messageUI?.Show("All systems restored. Proceeding…", 1.0f);

        if (monster) monster.Despawn();

        StartCoroutine(LoadNext());

        if (monster != null)
            monster.Despawn();
    }

    System.Collections.IEnumerator LoadNext()
    {
        yield return new WaitForSeconds(delayBeforeLoad);
        SceneManager.LoadScene(nextSceneName);
    }
}