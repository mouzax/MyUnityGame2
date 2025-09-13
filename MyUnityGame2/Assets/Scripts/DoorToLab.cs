using UnityEngine;
using UnityEngine.SceneManagement;

public class DoorToLab : MonoBehaviour
{
    [SerializeField] LockedDoor door;
    [SerializeField] string labScene = "Room2_Laboratory";

    void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Player")) return;
        if (door == null || !door.IsUnlocked) return;
        SceneManager.LoadScene(labScene);
    }
}
