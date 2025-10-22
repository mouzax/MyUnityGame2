using UnityEngine;

[RequireComponent(typeof(Collider))]
public class CollectItem : MonoBehaviour
{
    [Tooltip("Use exactly: Fuse, Valve, or Chip")]
    public string itemName = "Fuse";
    public string prompt = "Press SPACE to inspect";

    bool playerInRange;
    RoomShakeManager manager;

    void Start()
    {
        var col = GetComponent<Collider>();
        col.isTrigger = true;

        manager = FindObjectOfType<RoomShakeManager>();
        if (!manager) Debug.LogWarning("RoomShakeManager not found in scene.");
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerInRange = true;
            SceneMessage.Instance?.Show(prompt);
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerInRange = false;
            SceneMessage.Instance?.Hide();
        }
    }

    void Update()
    {
        if (!playerInRange) return;

        if (Input.GetKeyDown(KeyCode.Space))
        {
            SceneMessage.Instance?.Hide();
            SceneMessage.Instance?.ShowTemporary($"{itemName} collected");

            manager?.FoundItem(itemName);
            gameObject.SetActive(false);
        }
    }
}