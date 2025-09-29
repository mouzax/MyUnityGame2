using UnityEngine;

[RequireComponent(typeof(Camera))]
public class CameraClamp2D : MonoBehaviour
{
    [Header("Follow")]
    [SerializeField] private Transform target;          // Player
    [SerializeField] private Vector2 followOffset = Vector2.zero;
    [SerializeField] private float smoothTime = 0f;     // 0 = snap instantly

    [Header("Bounds (optional)")]
    [SerializeField] private SpriteRenderer roomSprite; // Drag background/room sprite here
    [SerializeField] private float padding = 0f;        // Padding inside the room edges

    [Header("Zoom")]
    [SerializeField] private float zoomSize = 3.5f;     // Smaller = more zoom

    private Camera cam;
    private Vector2 roomMin, roomMax;
    private Vector3 velocity;
    private bool hasBounds;

    private void Awake()
    {
        cam = GetComponent<Camera>();
        cam.orthographic = true;
        cam.orthographicSize = zoomSize;

        // Auto-find player if not set
        if (target == null)
        {
            GameObject player = GameObject.FindGameObjectWithTag("Player");
            if (player != null) target = player.transform;
        }

        ComputeBounds();
    }

    private void Start()
    {
        ComputeBounds();
    }

    private void ComputeBounds()
    {
        hasBounds = false;
        if (roomSprite == null) return;

        Bounds b = roomSprite.bounds;
        if (b.size.sqrMagnitude < 1e-6f) return;

        roomMin = (Vector2)b.min + new Vector2(padding, padding);
        roomMax = (Vector2)b.max - new Vector2(padding, padding);
        hasBounds = true;
    }

    private void LateUpdate()
    {
        if (target == null) return;

        // Camera should center on player + offset
        Vector3 desired = target.position + (Vector3)followOffset;
        desired.z = transform.position.z; // keep camera Z (usually -10)

        if (hasBounds)
        {
            float halfH = cam.orthographicSize;
            float halfW = halfH * cam.aspect;

            float roomW = roomMax.x - roomMin.x;
            float roomH = roomMax.y - roomMin.y;

            if (halfW >= roomW * 0.5f || halfH >= roomH * 0.5f)
            {
                // Camera view bigger than room → lock to center
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
        {
            transform.position = desired;
        }
        else
        {
            transform.position = Vector3.SmoothDamp(transform.position, desired, ref velocity, smoothTime);
        }
    }
}
