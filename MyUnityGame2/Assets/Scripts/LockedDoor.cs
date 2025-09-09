using UnityEngine;

public class LockedDoor : MonoBehaviour
{
    [SerializeField] KeyCode interactKey = KeyCode.Space;
    [SerializeField] string lockedMessage = "Locked! Requires passcode.";
    [SerializeField] UIMessage messageUI;

    bool playerInRange;
    bool unlocked = false;

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player")) playerInRange = true;
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player")) playerInRange = false;
    }

    void Update()
    {
        if (!playerInRange) return;

        if (Input.GetKeyDown(interactKey))
        {
            if (unlocked)
            {
                if (messageUI) messageUI.Show("Door opened!");
                gameObject.SetActive(false);
            }
            else
            {
                if (messageUI) messageUI.Show(lockedMessage, 2f);
            }
        }
    }

    public void Unlock()
    {
        unlocked = true;
        if (messageUI) messageUI.Show("Door is now unlocked!");
    }
}