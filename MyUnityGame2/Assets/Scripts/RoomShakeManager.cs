using UnityEngine;

public class RoomShakeManager : MonoBehaviour
{
    [Header("Required items to stop shaking")]
    public bool hasFuse = false;
    public bool hasValve = false;
    public bool hasChip = false;

    [Header("Shake Settings")]
    public float shakeDuration = 999f;
    public float shakeStrength = 0.6f;
    public float shakeRotation = 8f;

    bool shaking = false;

    void Start()
    {
        StartRoomShake();
    }

    void StartRoomShake()
    {
        if (!shaking)
        {
            shaking = true;
            CameraShake.Instance.Shake(shakeDuration, shakeStrength, shakeRotation);
        }
    }

    public void FoundItem(string itemName)
    {
        switch (itemName)
        {
            case "Fuse": hasFuse = true; break;
            case "Valve": hasValve = true; break;
            case "Chip": hasChip = true; break;
        }

        CheckIfAllFound();
    }

    void CheckIfAllFound()
    {
        if (hasFuse && hasValve && hasChip)
        {
            StopRoomShake();
        }
    }

    void StopRoomShake()
    {
        shaking = false;
        CameraShake.Instance.Shake(0f, 0f);
        Debug.Log("Room stabilized ✅");
    }
}