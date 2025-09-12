using UnityEngine;

public class LockedDoor : MonoBehaviour
{
    [SerializeField] KeyCode interactKey = KeyCode.Space;
    [SerializeField] UIMessage messageUI;
    [SerializeField] GameObject keypadPanel;
    [SerializeField] Collider2D blockingCollider;
    [SerializeField] PasscodeManager passcode;
    [SerializeField] Mission1State mission1;

    bool playerInRange;
    bool unlocked = false;

    void OnTriggerEnter2D(Collider2D other){ if (other.CompareTag("Player")) playerInRange = true; }
    void OnTriggerExit2D(Collider2D other){ if (other.CompareTag("Player")) playerInRange = false; }

    void Update()
    {
        if (!playerInRange) return;

        if (Input.GetKeyDown(interactKey))
        {
            if (unlocked)
            {
                OpenDoor();
                return;
            }

        mission1?.MarkDoorTried();

        if (passcode != null && passcode.AllRevealed())
        {
            keypadPanel?.SetActive(true);
        }
        else
        {
            if (passcode != null)
                messageUI?.Show($"Locked. Known: {passcode.GetKnownPattern('•')}", 2f);
            else
                messageUI?.Show("Locked. Requires passcode.", 2f);
        }
    }
}

    public void Unlock()
    {
        unlocked = true;
        messageUI?.Show("Door unlocked.");
        OpenDoor();
    }

    void OpenDoor()
    {
        if (blockingCollider) blockingCollider.enabled = false;
    }
}