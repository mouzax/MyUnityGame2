using UnityEngine;

public enum ItemType { Valve, Chip, Fuse }

public class CollectibleItem : MonoBehaviour
{
    [Header("Setup")]
    public ItemType itemType;
    public string displayName = "Item";
    public bool destroyOnCollect = false;

    public void OnCollected()
    {
        if (destroyOnCollect) Destroy(gameObject);
        else gameObject.SetActive(false);
    }
}