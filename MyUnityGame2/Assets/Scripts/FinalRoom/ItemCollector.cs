using UnityEngine;
using System.Collections.Generic;

public class ItemCollector : MonoBehaviour
{
    [Header("Detection")]
    [SerializeField] float interactRadius = 2.0f;

    [Header("References")]
    [SerializeField] MessageUI messageUI;
    [SerializeField] RoomShaker roomShaker;
    [SerializeField] EndGameUI endGameUI;

    HashSet<ItemType> collected = new HashSet<ItemType>();
    CollectibleItem nearestItem;

    void Update()
    {
        nearestItem = FindNearestItemAnyPhysics();

        bool canInteract = nearestItem != null && nearestItem.gameObject.activeInHierarchy;
        if (messageUI) messageUI.ShowPrompt(canInteract);

        if (canInteract && Input.GetKeyDown(KeyCode.Space))
        {
            Collect(nearestItem);
        }
    }

    CollectibleItem FindNearestItemAnyPhysics()
    {
        float bestDist = Mathf.Infinity;
        CollectibleItem best = null;

        Collider[] hits3D = Physics.OverlapSphere(transform.position, interactRadius);
        foreach (var h in hits3D)
        {
            var item = h.GetComponentInParent<CollectibleItem>() ?? h.GetComponent<CollectibleItem>();
            if (item == null || !item.gameObject.activeInHierarchy) continue;

            float d = Vector3.Distance(transform.position, item.transform.position);
            if (d < bestDist)
            {
                bestDist = d;
                best = item;
            }
        }

        Collider2D[] hits2D = Physics2D.OverlapCircleAll(transform.position, interactRadius);
        foreach (var h in hits2D)
        {
            var item = h.GetComponentInParent<CollectibleItem>() ?? h.GetComponent<CollectibleItem>();
            if (item == null || !item.gameObject.activeInHierarchy) continue;

            float d = Vector3.Distance(transform.position, item.transform.position);
            if (d < bestDist)
            {
                bestDist = d;
                best = item;
            }
        }

        return best;
    }

    void Collect(CollectibleItem item)
    {
        collected.Add(item.itemType);
        item.OnCollected();

        if (messageUI)
            messageUI.ShowCollected(item.displayName + " Collected");

        if (HasAllItems())
        {
            if (roomShaker != null)
                roomShaker.StopShake();

            if (endGameUI != null)
                endGameUI.ShowWin(
                    "Escape Successful!",
                    "You restored power, turn switches, collected all components, and escaped the facility!"
                );
        }
    }

    bool HasAllItems()
    {
        return collected.Contains(ItemType.Chip)
            && collected.Contains(ItemType.Fuse)
            && collected.Contains(ItemType.Valve);
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, interactRadius);
    }
}