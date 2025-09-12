using UnityEngine;

public class ElectricBox : MonoBehaviour
{
    [SerializeField] LightingController lighting; 
    [SerializeField] KeyCode interactKey = KeyCode.Space;

    bool playerInRange;

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
        if (!playerInRange) return;

        if (Input.GetKeyDown(interactKey) && lighting)
        {
            lighting.TurnPowerOn();
        }
    }
}
