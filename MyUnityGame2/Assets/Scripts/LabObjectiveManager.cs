using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;
using System.Linq;
using System.Collections;

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

    private SwitchableLight[] lights;
    private int total;
    private int remainingOn;

    void OnEnable()
    {
        LabInteractable.OnActivatedGlobal += HandleInteractableActivated;
        MonsterController.OnPlayerCaught += HandlePlayerCaught;
    }

    void OnDisable()
    {
        LabInteractable.OnActivatedGlobal -= HandleInteractableActivated;
        MonsterController.OnPlayerCaught -= HandlePlayerCaught;
    }

    void Start()
    {
    #if UNITY_2023_1_OR_NEWER
        lights = Object.FindObjectsByType<SwitchableLight>(FindObjectsSortMode.None);
    #else
        lights = FindObjectsOfType<SwitchableLight>(includeInactive: false);
    #endif

        total = lights.Length;
        remainingOn = lights.Count(l => l.IsOn);

        foreach (var l in lights)
            l.OnTurnedOff.AddListener(HandleLightTurnedOff);

        UpdateProgressUI();

        if (spawnPoint == null)
        {
            var tagObj = GameObject.FindGameObjectWithTag("MonsterSpawn");
            if (tagObj != null) spawnPoint = tagObj.transform;
            else
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

    void HandleInteractableActivated(LabInteractable li)
    {

    }

    void HandleLightTurnedOff()
    {
        remainingOn = Mathf.Max(0, remainingOn - 1);
        UpdateProgressUI();

        if (remainingOn == 0)
            CompleteObjective();
    }

    void HandlePlayerCaught()
    {
        foreach (var l in lights) l.ForceOn();
        remainingOn = total;
        UpdateProgressUI();

        if (messageUI != null)
            messageUI.Show("The monster caught you! Resetting…", 1.2f);
    }

    void UpdateProgressUI()
    {
        if (progressText != null)
            progressText.text = $"Lights Remaining: {remainingOn}/{total}";
    }

    void CompleteObjective()
    {
        if (messageUI != null) messageUI.Show("All lights switched off. Proceeding…", 1.0f);

        if (monster != null) monster.Despawn();

        StartCoroutine(LoadNext());
    }

    IEnumerator LoadNext()
    {
        yield return new WaitForSeconds(delayBeforeLoad);
        SceneManager.LoadScene(nextSceneName);
    }

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
}