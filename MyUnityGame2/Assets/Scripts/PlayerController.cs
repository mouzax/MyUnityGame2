using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(BoxCollider2D))]
[RequireComponent(typeof(SpriteRenderer))]
public class PlayerController : MonoBehaviour
{
    [Header("Movement")]
    [SerializeField] float moveSpeed = 5f;

    [Header("Confinement (draw over the brown floor)")]
    [SerializeField] PolygonCollider2D walkArea;
    [SerializeField] float skin = 0.015f;

    [Header("Grab / Toggle (SPACE)")]
    [SerializeField] Transform handAnchor;
    [SerializeField] float grabRadius = 0.7f;
    [SerializeField] LayerMask grabbableMask = ~0;
    [SerializeField] bool disableHeldCollider = true;
    [SerializeField] float dropNudge = 0.5f;

    [Header("Switch Activation (SPACE)")]
    [SerializeField] LayerMask switchMask = ~0;
    [SerializeField] bool preferSwitchOverGrab = true;

    Rigidbody2D rb;
    BoxCollider2D box;
    SpriteRenderer sr;

    Vector2 input;
    int lastLookDir = 1;

    Rigidbody2D heldRb;
    Collider2D  heldCol;
    Transform   heldTf;

    void Awake()
    {
        rb  = GetComponent<Rigidbody2D>();
        box = GetComponent<BoxCollider2D>();
        sr  = GetComponent<SpriteRenderer>();

        rb.gravityScale = 0f;
        rb.freezeRotation = true;
        rb.collisionDetectionMode = CollisionDetectionMode2D.Continuous;
        rb.interpolation = RigidbodyInterpolation2D.Interpolate;

        var mat = new PhysicsMaterial2D("NoFriction") { friction = 0f, bounciness = 0f };
        box.sharedMaterial = mat;

        if (handAnchor == null)
        {
            var go = new GameObject("Hand");
            go.transform.SetParent(transform);
            go.transform.localPosition = new Vector3(0.25f, 0f, 0f);
            handAnchor = go.transform;
        }
    }

    void Update()
    {
        float h = Input.GetAxisRaw("Horizontal");
        float v = Input.GetAxisRaw("Vertical");
        input = new Vector2(h, v);
        if (input.sqrMagnitude > 1f) input.Normalize();

        if (h > 0.01f) lastLookDir = 1;
        else if (h < -0.01f) lastLookDir = -1;
        sr.flipX = (lastLookDir == -1);

        if (Input.GetKeyDown(KeyCode.Space))
        {
            if (preferSwitchOverGrab)
            {
                if (TryActivateNearest()) return;
                if (heldTf == null) TryPickup();
                else Drop();
            }
            else
            {
                if (heldTf == null) TryPickup();
                else Drop();
                if (TryActivateNearest()) return;
            }
        }

        if (heldTf != null) heldTf.position = handAnchor.position;
    }

    void FixedUpdate()
    {
        Vector2 pos   = rb.position;
        Vector2 delta = input * moveSpeed * Time.fixedDeltaTime;

        if (walkArea != null && CanFitInside(pos + delta))
        {
            rb.MovePosition(pos + delta);
            return;
        }

        if (walkArea != null)
        {
            Vector2 edgeA, edgeB;
            if (TryGetNearestEdge(walkArea, pos + delta, out edgeA, out edgeB))
            {
                Vector2 edgeDir = (edgeB - edgeA).normalized;
                Vector2 slide   = Vector2.Dot(delta, edgeDir) * edgeDir;
                if (TrySlide(pos, slide)) return;
            }
        }

        if (TrySlide(pos, new Vector2(delta.x, 0f))) return;
        if (TrySlide(pos, new Vector2(0f, delta.y))) return;
    }

    bool TryActivateNearest()
    {
        var hits = Physics2D.OverlapCircleAll(transform.position, grabRadius,
            switchMask.value == 0 ? ~0 : switchMask);

        LabSwitch closest = null;
        float bestDist = float.PositiveInfinity;

        foreach (var h in hits)
        {
            if (h == null || !h.gameObject.activeInHierarchy) continue;

            var sw = h.GetComponent<LabSwitch>();
            if (sw == null) continue;
            if (sw.IsOn) continue;

            float d = ((Vector2)h.transform.position - (Vector2)handAnchor.position).sqrMagnitude;
            if (d < bestDist) { bestDist = d; closest = sw; }
        }

        if (closest != null)
        {
            closest.Activate();
            return true;
        }
        return false;
    }

    void TryPickup()
    {
        var hits = Physics2D.OverlapCircleAll(transform.position, grabRadius, grabbableMask);

        float bestDist = float.PositiveInfinity;
        Collider2D best = null;

        foreach (var col in hits)
        {
            if (col == null || col.attachedRigidbody == rb) continue;
            if (!col.gameObject.activeInHierarchy) continue;

            if (col.GetComponent<Grabbable>() == null) continue;

            float d = ((Vector2)col.transform.position - (Vector2)handAnchor.position).sqrMagnitude;
            if (d < bestDist) { bestDist = d; best = col; }
        }

        if (best == null) return;

        heldCol = best;
        heldRb  = best.attachedRigidbody;
        heldTf  = best.transform;

        heldTf.SetParent(handAnchor);
        heldTf.position = handAnchor.position;

        if (heldRb != null)
        {
            heldRb.linearVelocity = Vector2.zero;
            heldRb.angularVelocity = 0f;
            heldRb.bodyType = RigidbodyType2D.Kinematic;
        }
        if (disableHeldCollider && heldCol != null) heldCol.enabled = false;
    }

    void Drop()
    {
        if (heldTf == null) return;

        heldTf.SetParent(null);

        if (heldRb != null)
        {
            heldRb.bodyType = RigidbodyType2D.Dynamic;
            Vector2 push = (input.sqrMagnitude > 0.01f ? input : new Vector2(lastLookDir, 0f)) * dropNudge;
            heldRb.linearVelocity = push / Mathf.Max(Time.fixedDeltaTime, 0.0001f);
        }
        if (heldCol != null && disableHeldCollider) heldCol.enabled = true;

        heldTf = null; heldRb = null; heldCol = null;
    }

    bool TrySlide(Vector2 from, Vector2 slide)
    {
        if (slide == Vector2.zero) return false;

        Vector2 target = from + slide;
        if (walkArea == null || CanFitInside(target)) { rb.MovePosition(target); return true; }

        float t = 0.8f;
        for (int i = 0; i < 3; i++)
        {
            target = from + slide * t;
            if (CanFitInside(target)) { rb.MovePosition(target); return true; }
            t *= 0.5f;
        }
        return false;
    }

    bool CanFitInside(Vector2 center)
    {
        Vector2 half = Vector2.Scale(box.size * 0.5f, transform.lossyScale);
        Vector2 off  = box.offset;
        Vector2 c    = center + off;

        Vector2 p1 = new Vector2(c.x - half.x + skin, c.y - half.y + skin);
        Vector2 p2 = new Vector2(c.x + half.x - skin, c.y - half.y + skin);
        Vector2 p3 = new Vector2(c.x - half.x + skin, c.y + half.y - skin);
        Vector2 p4 = new Vector2(c.x + half.x - skin, c.y + half.y - skin);

        return walkArea.OverlapPoint(p1) &&
               walkArea.OverlapPoint(p2) &&
               walkArea.OverlapPoint(p3) &&
               walkArea.OverlapPoint(p4);
    }

    bool TryGetNearestEdge(PolygonCollider2D poly, Vector2 point, out Vector2 bestA, out Vector2 bestB)
    {
        bestA = bestB = default;
        float bestDist = float.PositiveInfinity;

        int pathCount = poly.pathCount;
        for (int p = 0; p < pathCount; p++)
        {
            var path = poly.GetPath(p);
            int n = path.Length;
            if (n < 2) continue;

            for (int i = 0; i < n; i++)
            {
                Vector2 a = poly.transform.TransformPoint(path[i]);
                Vector2 b = poly.transform.TransformPoint(path[(i + 1) % n]);
                float d = DistancePointToSegment(point, a, b, out _);
                if (d < bestDist) { bestDist = d; bestA = a; bestB = b; }
            }
        }
        return bestDist < float.PositiveInfinity;
    }

    float DistancePointToSegment(Vector2 p, Vector2 a, Vector2 b, out Vector2 closest)
    {
        Vector2 ab = b - a;
        float t = Vector2.Dot(p - a, ab) / Mathf.Max(ab.sqrMagnitude, 1e-8f);
        t = Mathf.Clamp01(t);
        closest = a + t * ab;
        return Vector2.Distance(p, closest);
    }

    void OnDrawGizmosSelected()
    {
        if (handAnchor != null)
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(handAnchor.position, grabRadius);
        }
    }
}