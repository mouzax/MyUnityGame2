using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(Collider2D))]
public class ItemConfinement2D : MonoBehaviour
{
    [Header("Room Boundary")]
    [SerializeField] PolygonCollider2D walkArea;
    [SerializeField] float skin = 0.015f;

    Rigidbody2D rb;
    Collider2D col;
    CircleCollider2D circle;
    BoxCollider2D box;

    void Awake()
    {
        rb     = GetComponent<Rigidbody2D>();
        col    = GetComponent<Collider2D>();
        circle = col as CircleCollider2D;
        box    = col as BoxCollider2D;

        rb.gravityScale = 0f;
        rb.collisionDetectionMode = CollisionDetectionMode2D.Continuous;
        rb.interpolation = RigidbodyInterpolation2D.Interpolate;
        rb.freezeRotation = true;

        var mat = new PhysicsMaterial2D("NoFriction") { friction = 0f, bounciness = 0f };
        col.sharedMaterial = mat;
    }

    void FixedUpdate()
    {
        float dt = Time.fixedDeltaTime;

        Vector2 pos = rb.position;
        Vector2 vel = rb.linearVelocity;
        Vector2 next = pos + vel * dt;

        if (FitsInside(next))
        {
            return;
        }

        Vector2 edge = walkArea.ClosestPoint(next);
        Vector2 n = (next - edge);
        if (n.sqrMagnitude < 1e-8f) n = (pos - edge); // fallback if exactly on edge
        if (n.sqrMagnitude < 1e-8f) n = Vector2.up;
        n.Normalize();

        Vector2 inwardOffset = GetInwardOffset(n);
        Vector2 correctedPos = edge + inwardOffset;

        Vector2 tangentVel = vel - Vector2.Dot(vel, n) * n;

        rb.position = correctedPos;
        rb.linearVelocity = tangentVel;
    }

    Vector2 GetInwardOffset(Vector2 n)
    {
        if (circle != null)
        {
            float r = circle.radius * Mathf.Max(transform.lossyScale.x, transform.lossyScale.y);
            return n * (r + skin);
        }
        else if (box != null)
        {
            Vector2 half = Vector2.Scale(box.size * 0.5f, transform.lossyScale);
            float proj = Mathf.Abs(n.x) * half.x + Mathf.Abs(n.y) * half.y;
            return n * (proj + skin);
        }
        else
        {
            return n * (0.2f + skin);
        }
    }

    bool FitsInside(Vector2 center)
    {
        if (circle != null)
        {
            float r = circle.radius * Mathf.Max(transform.lossyScale.x, transform.lossyScale.y);
            return Inside(center) &&
                   Inside(center + Vector2.right * (r - skin)) &&
                   Inside(center + Vector2.left  * (r - skin)) &&
                   Inside(center + Vector2.up    * (r - skin)) &&
                   Inside(center + Vector2.down  * (r - skin));
        }
        else if (box != null)
        {
            Vector2 half = Vector2.Scale(box.size * 0.5f, transform.lossyScale);
            Vector2 off  = box.offset;

            Vector2 c = center + off;
            Vector2 p1 = new Vector2(c.x - half.x + skin, c.y - half.y + skin); // BL
            Vector2 p2 = new Vector2(c.x + half.x - skin, c.y - half.y + skin); // BR
            Vector2 p3 = new Vector2(c.x - half.x + skin, c.y + half.y - skin); // TL
            Vector2 p4 = new Vector2(c.x + half.x - skin, c.y + half.y - skin); // TR

            return Inside(p1) && Inside(p2) && Inside(p3) && Inside(p4);
        }
        else
        {
            return Inside(center);
        }
    }

    bool Inside(Vector2 p) => walkArea.OverlapPoint(p);

#if UNITY_EDITOR
    void OnDrawGizmosSelected()
    {
        if (walkArea == null) return;
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(transform.position, 0.08f);
    }
#endif
}