using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(BoxCollider2D))]
[RequireComponent(typeof(SpriteRenderer))]
public class PlayerController : MonoBehaviour
{
    [Header("Movement")]
    [SerializeField] float moveSpeed = 5f;

    [Header("Confinement (draw over the brown floor)")]
    [SerializeField] PolygonCollider2D walkArea;   // Must be assigned in Inspector, Is Trigger = ON
    [SerializeField] float skin = 0.015f;          // tiny inward padding to avoid snagging

    Rigidbody2D rb;
    BoxCollider2D box;
    SpriteRenderer sr;

    Vector2 input;
    int lastLookDir = 1; // 1=right, -1=left

    void Awake()
    {
        rb  = GetComponent<Rigidbody2D>();
        box = GetComponent<BoxCollider2D>();
        sr  = GetComponent<SpriteRenderer>();

        // Top-down RB settings
        rb.gravityScale = 0f;
        rb.freezeRotation = true;
        rb.collisionDetectionMode = CollisionDetectionMode2D.Continuous;
        rb.interpolation = RigidbodyInterpolation2D.Interpolate;

        // Optional: zero-friction mat so edges feel smooth
        var mat = new PhysicsMaterial2D("NoFriction") { friction = 0f, bounciness = 0f };
        box.sharedMaterial = mat;
    }

    void FixedUpdate()
    {
        // --- Input ---
        float h = Input.GetAxisRaw("Horizontal");
        float v = Input.GetAxisRaw("Vertical");
        input = new Vector2(h, v);
        if (input.sqrMagnitude > 1f) input.Normalize();

        // --- Flip sprite & remember facing when idle ---
        if (h > 0.01f) lastLookDir = 1;
        else if (h < -0.01f) lastLookDir = -1;
        sr.flipX = (lastLookDir == -1);

        // --- Movement with polygon confinement & smooth slide ---
        Vector2 pos   = rb.position;
        Vector2 delta = input * moveSpeed * Time.fixedDeltaTime;

        // 1) Try full move
        if (CanFitInside(pos + delta)) { rb.MovePosition(pos + delta); return; }

        // 2) Slide along nearest polygon edge (true tangent slide)
        Vector2 edgeA, edgeB;
        if (TryGetNearestEdge(walkArea, pos + delta, out edgeA, out edgeB))
        {
            Vector2 edgeDir = (edgeB - edgeA).normalized;
            Vector2 slide   = Vector2.Dot(delta, edgeDir) * edgeDir;
            if (TrySlide(pos, slide)) return;
        }

        // 3) Fallback: axis slides (X then Y) for tight corners
        if (TrySlide(pos, new Vector2(delta.x, 0f))) return;
        if (TrySlide(pos, new Vector2(0f, delta.y))) return;
        // 4) Blocked: stay put
    }

    bool TrySlide(Vector2 from, Vector2 slide)
    {
        if (slide == Vector2.zero) return false;

        Vector2 target = from + slide;
        if (CanFitInside(target)) { rb.MovePosition(target); return true; }

        // Nudge down the distance a few times to hug corners
        float t = 0.8f;
        for (int i = 0; i < 3; i++)
        {
            target = from + slide * t;
            if (CanFitInside(target)) { rb.MovePosition(target); return true; }
            t *= 0.5f;
        }
        return false;
    }

    // Ensure the whole BoxCollider2D fits inside the polygon at 'center'
    bool CanFitInside(Vector2 center)
    {
        // assumes Z rotation frozen (box not rotated)
        Vector2 half = Vector2.Scale(box.size * 0.5f, transform.lossyScale);
        Vector2 off  = box.offset;
        Vector2 c    = center + off;

        Vector2 p1 = new Vector2(c.x - half.x + skin, c.y - half.y + skin); // BL
        Vector2 p2 = new Vector2(c.x + half.x - skin, c.y - half.y + skin); // BR
        Vector2 p3 = new Vector2(c.x - half.x + skin, c.y + half.y - skin); // TL
        Vector2 p4 = new Vector2(c.x + half.x - skin, c.y + half.y - skin); // TR

        return walkArea.OverlapPoint(p1) &&
               walkArea.OverlapPoint(p2) &&
               walkArea.OverlapPoint(p3) &&
               walkArea.OverlapPoint(p4);
    }

    // Find nearest polygon edge (world space) to a point
    bool TryGetNearestEdge(PolygonCollider2D poly, Vector2 point, out Vector2 bestA, out Vector2 bestB)
    {
        bestA = bestB = default;
        float bestDist = float.PositiveInfinity;

        int pathCount = poly.pathCount;
        for (int p = 0; p < pathCount; p++)
        {
            var path = poly.GetPath(p);  // local space points
            int n = path.Length;
            if (n < 2) continue;

            for (int i = 0; i < n; i++)
            {
                Vector2 a = poly.transform.TransformPoint(path[i]);
                Vector2 b = poly.transform.TransformPoint(path[(i + 1) % n]);
                float d = DistancePointToSegment(point, a, b, out _);
                if (d < bestDist)
                {
                    bestDist = d; bestA = a; bestB = b;
                }
            }
        }
        return bestDist < float.PositiveInfinity;
    }

    // Distance from point to segment (also returns closest point)
    float DistancePointToSegment(Vector2 p, Vector2 a, Vector2 b, out Vector2 closest)
    {
        Vector2 ab = b - a;
        float t = Vector2.Dot(p - a, ab) / Mathf.Max(ab.sqrMagnitude, 1e-8f);
        t = Mathf.Clamp01(t);
        closest = a + t * ab;
        return Vector2.Distance(p, closest);
    }
}
