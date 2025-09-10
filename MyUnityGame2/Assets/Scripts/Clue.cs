using UnityEngine;

public class Clue : MonoBehaviour
{
    [Header("Clue Setup")]
    [SerializeField] PasscodeManager manager;
    [SerializeField] int digitIndex = 0;

    [Header("UI Prompt")]
    [SerializeField] UIMessage messageUI;
    [SerializeField] string inspectHint = "Press Space to inspect";
    [SerializeField] Mission1State mission1;
    [SerializeField] string blockedMsgBeforePower = "It's too dark to search.";
    [SerializeField] string blockedMsgBeforeDoor  = "Check the lab door first.";

    bool playerInRange;
    bool used;

    void Reset()
    {
        var col = GetComponent<Collider2D>();
        if (!col) col = gameObject.AddComponent<BoxCollider2D>();
        col.isTrigger = true;
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            playerInRange = true;
            if (messageUI && !used && mission1 && mission1.CanSearchClues)
                messageUI.Show(inspectHint, 1.2f);
        }
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player")) playerInRange = false;
    }

    void Update()
    {
        if (!playerInRange || used || manager == null) return;

        if (Input.GetKeyDown(KeyCode.Space))
        {
            if (mission1 && !mission1.PowerOn) return;
            if (mission1 && !mission1.DoorTried) return;
            used = true;
            manager.RevealDigit(digitIndex);

            var sr = GetComponent<SpriteRenderer>();
            if (sr) sr.color = new Color(sr.color.r, sr.color.g, sr.color.b, 0.5f);

            messageUI?.Show($"Known: {manager.GetKnownPattern('•')}", 1.5f);
        }
    }
}