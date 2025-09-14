using UnityEngine;

[RequireComponent(typeof(Camera))]
public class CameraClamp2D : MonoBehaviour
{
    [Header("Follow")]
    [SerializeField] Transform target;                  // Player
    [SerializeField] Vector2 followOffset = Vector2.zero;
    [SerializeField] float smoothTime = 0f;             // 0 = perfect center snap

    [Header("Bounds (optional)")]
    [SerializeField] SpriteRenderer roomSprite;         // Big background sprite
    [SerializeField] float padding = 0f;

    Camera cam;
    Vector2 roomMin, roomMax;
    Vector3 velocity;
    bool hasBounds;

    void Awake()
    {
        cam = GetComponent<Camera>();
        cam.orthographic = true;

        // Always try to bind target
        if (!target)
        {
            var go = GameObject.FindGameObjectWithTag("Player");
            if (go) target = go.transform;
        }

        ComputeBounds();
    }

    void Start() { ComputeBounds(); }

    void ComputeBounds()
    {
        hasBounds = false;
        if (!roomSprite) return;

        Bounds b = roomSprite.bounds;
        if (b.size.sqrMagnitude < 1e-6f) return;

        roomMin = (Vector2)b.min + new Vector2(padding, padding);
        roomMax = (Vector2)b.max - new Vector2(padding, padding);
        hasBounds = true;
    }

    void LateUpdate()
    {
        if (!target) return;

        // Desired center = player (+ optional offset)
        Vector3 desired = target.position + (Vector3)followOffset;
        desired.z = transform.position.z; // keep camera Z

        if (hasBounds)
        {
            float halfH = cam.orthographicSize;
            float halfW = halfH * cam.aspect;

            float roomW = roomMax.x - roomMin.x;
            float roomH = roomMax.y - roomMin.y;

            if (halfW >= roomW * 0.5f || halfH >= roomH * 0.5f)
            {
                // Camera view bigger than room → center lock
                Vector2 center = (roomMin + roomMax) * 0.5f;
                desired.x = center.x;
                desired.y = center.y;
            }
            else
            {
                desired.x = Mathf.Clamp(desired.x, roomMin.x + halfW, roomMax.x - halfW);
                desired.y = Mathf.Clamp(desired.y, roomMin.y + halfH, roomMax.y - halfH);
            }
        }

        if (smoothTime <= 0.0001f)
            transform.position = desired; // perfect center snap
        else
            transform.position = Vector3.SmoothDamp(transform.position, desired, ref velocity, smoothTime);
    }
}