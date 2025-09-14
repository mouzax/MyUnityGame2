using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class LabObjectiveManager : MonoBehaviour
{
    [Header("Targets & Hooks")]
    [SerializeField] private MonsterController monster;
    [SerializeField] private UIMessage messageUI;
    [SerializeField] private TMP_Text progressText;

    [Header("Monster Spawn")]
    [SerializeField] private Transform spawnPoint;
    [SerializeField] private GameObject objectPrefab;

    [Header("Next Scene")]
    [SerializeField] private string nextSceneName = "Main";
    [SerializeField] private float delayBeforeLoad = 1.2f;

    private LabInteractable[] interactables;
    private int total;
    private int activatedCount;

    void OnEnable()  { LabInteractable.OnActivatedGlobal += HandleActivated; }
    void OnDisable() { LabInteractable.OnActivatedGlobal -= HandleActivated; }

    public void SpawnObject()
    {
        if (objectPrefab != null && spawnPoint != null)
        {
            Instantiate(objectPrefab, spawnPoint.position, spawnPoint.rotation);
        }
        else
        {
            Debug.LogWarning("[LabObjectiveManager] SpawnObject: Missing prefab or spawnPoint.");
        }
    }

    void Start()
    {
    #if UNITY_2023_1_OR_NEWER
        interactables = Object.FindObjectsByType<LabInteractable>(FindObjectsSortMode.None);
    #else
        interactables = FindObjectsOfType<LabInteractable>(includeInactive: false);
    #endif

        total = 0;
        activatedCount = 0;

        foreach (var li in interactables)
        {
            if (li.Type == LabThing.Microscope ||
                li.Type == LabThing.GooTank ||
                li.Type == LabThing.Computer ||
                li.Type == LabThing.Flask)
            {
                total++;
                if (li.Activated) activatedCount++;
            }
        }
        UpdateProgressUI();

        if (spawnPoint == null)
        {
            var tagObj = GameObject.FindGameObjectWithTag("MonsterSpawn");
            if (tagObj != null) spawnPoint = tagObj.transform;
            if (spawnPoint == null)
            {
                var byName = GameObject.Find("MonsterSpawn");
                if (byName != null) spawnPoint = byName.transform;
            }
        }

        if (monster != null && spawnPoint != null)
        {
            monster.Spawn(spawnPoint.position);
        }
        else if (monster == null)
        {
            Debug.LogWarning("[LabObjectiveManager] Monster reference is missing.");
        }
        else if (spawnPoint == null)
        {
            Debug.LogWarning("[LabObjectiveManager] spawnPoint is not assigned and no fallback found. Assign it in the Inspector.");
        }
    }

    void HandleActivated(LabInteractable li)
    {
        if (li.Type != LabThing.Microscope &&
            li.Type != LabThing.GooTank &&
            li.Type != LabThing.Computer &&
            li.Type != LabThing.Flask) return;

        activatedCount = Mathf.Clamp(activatedCount + 1, 0, total);
        UpdateProgressUI();

        if (activatedCount >= total)
            CompleteObjective();
    }

    void UpdateProgressUI()
    {
        if (progressText != null)
            progressText.text = $"Systems Restored: {activatedCount}/{total}";
    }

    void CompleteObjective()
    {
        if (messageUI != null) messageUI.Show("All systems restored. Proceeding…", 1.0f);

        if (monster != null) monster.Despawn();

        StartCoroutine(LoadNext());
    }

    System.Collections.IEnumerator LoadNext()
    {
        yield return new WaitForSeconds(delayBeforeLoad);
        SceneManager.LoadScene(nextSceneName);
    }
}