using UnityEngine;

public class ComputerTerminal : MonoBehaviour
{
    [Header("UI")]
    [SerializeField] GameObject mazePanel;

    [Header("Optional gating (set & assign Mission1State if used)")]
    [SerializeField] bool requirePowerOn = false;
    [SerializeField] bool requireDoorTried = false;
    [SerializeField] Mission1State mission1;

    [Header("Input")]
    [SerializeField] KeyCode interactKey = KeyCode.Space;

    bool playerInRange;
    bool completed = false;

    void Reset()
    {
        var col = GetComponent<Collider2D>();
        if (!col) col = gameObject.AddComponent<BoxCollider2D>();
        col.isTrigger = true;
    }

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
        if (!playerInRange || completed) return;

        if (Input.GetKeyDown(interactKey))
        {
            if (!UseGate()) return;
            if (mazePanel) mazePanel.SetActive(true);
        }
    }

    bool UseGate()
    {
        if (!mission1) return !(requirePowerOn || requireDoorTried);
        if (requirePowerOn   && !mission1.PowerOn)    return false;
        if (requireDoorTried && !mission1.DoorTried)  return false;
        return true;
    }

    public void MarkCompleted()
    {
        completed = true;

        var col = GetComponent<Collider2D>();
        if (col) col.enabled = false;

        this.enabled = false;

        var sr = GetComponentInChildren<SpriteRenderer>();
        if (sr) sr.color = new Color(sr.color.r, sr.color.g, sr.color.b, 0.5f);
    }
}