using UnityEngine;
using UnityEngine.SceneManagement;

public class DoorToLab : MonoBehaviour
{
    [SerializeField] LockedDoor door;
    [SerializeField] string labScene = "Room2_Laboratory";
    
    //Audio
    [SerializeField] AudioSource sfxSource;
    [SerializeField] AudioClip successSFX;

    void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Player")) return;
        if (door == null || !door.IsUnlocked) return;
        SceneManager.LoadScene(labScene);

        //Audio
        sfxSource.PlayOneShot(successSFX);
    }
}
