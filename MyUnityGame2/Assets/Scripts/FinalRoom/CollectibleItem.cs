using UnityEngine;

public enum ItemType { Valve, Chip, Fuse }

public class CollectibleItem : MonoBehaviour
{
    [Header("Setup")]
    public ItemType itemType;
    public string displayName = "Item";
    public bool destroyOnCollect = false;

    [Header("Audio")]
    [SerializeField] AudioSource sfxSource;
    [SerializeField] AudioClip pickupSFX;

    public void OnCollected()
    {

        //Audio
        if (sfxSource != null && pickupSFX != null)
            sfxSource.PlayOneShot(pickupSFX);

        if (destroyOnCollect)
            Destroy(gameObject);
        else
            gameObject.SetActive(false);

        
        if (destroyOnCollect) Destroy(gameObject);
        else gameObject.SetActive(false);
    }
}