using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
public class PlayerController : MonoBehaviour
{
    [SerializeField] float moveSpeed = 5f;
    [SerializeField] float padding = 0.1f;

    Camera cam;
    float halfW, halfH;

    void Awake()
    {
        cam = Camera.main;

        var sr = GetComponent<SpriteRenderer>();
        if (sr != null)
        {
            halfW = sr.bounds.extents.x;
            halfH = sr.bounds.extents.y;
        }
    }

    void Update()
    {
        float h = Input.GetAxisRaw("Horizontal");
        float v = Input.GetAxisRaw("Vertical");

        Vector3 dir = new Vector3(h, v, 0f).normalized;
        Vector3 pos = transform.position + dir * moveSpeed * Time.deltaTime;

        float camHeight = cam.orthographicSize * 2f;
        float camWidth  = camHeight * cam.aspect;

        float minX = cam.transform.position.x - camWidth  / 2f + halfW + padding;
        float maxX = cam.transform.position.x + camWidth  / 2f - halfW - padding;
        float minY = cam.transform.position.y - camHeight / 2f + halfH + padding;
        float maxY = cam.transform.position.y + camHeight / 2f - halfH - padding;

        pos.x = Mathf.Clamp(pos.x, minX, maxX);
        pos.y = Mathf.Clamp(pos.y, minY, maxY);

        transform.position = pos;
    }
}
