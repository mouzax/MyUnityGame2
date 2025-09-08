using UnityEngine;

[RequireComponent(typeof(Camera))]
public class CameraClamp2D : MonoBehaviour
{
    public Transform target;
    public SpriteRenderer roomSprite;

    Camera cam;
    Vector2 roomMin, roomMax;

    void Awake()
    {
        cam = GetComponent<Camera>();

        Bounds b = roomSprite.bounds;
        roomMin = b.min;
        roomMax = b.max;

        float roomWidth  = b.size.x;
        float roomHeight = b.size.y;
        float aspect = cam.aspect;

        float sizeByHeight = roomHeight * 0.5f;
        float sizeByWidth  = (roomWidth  * 0.5f) / aspect;

        cam.orthographicSize = Mathf.Min(sizeByHeight, sizeByWidth);
    }

    void LateUpdate()
    {
        if (!target) return;

        float halfH = cam.orthographicSize;
        float halfW = halfH * cam.aspect;

        float x = Mathf.Clamp(target.position.x, roomMin.x + halfW, roomMax.x - halfW);
        float y = Mathf.Clamp(target.position.y, roomMin.y + halfH, roomMax.y - halfH);

        transform.position = new Vector3(x, y, transform.position.z);
    }
}