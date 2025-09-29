using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class MonsterChaser : MonoBehaviour
{
    [Header("Target")]
    [SerializeField] Transform player; 
    [SerializeField] Transform playerRespawnPoint; 

    [Header("Movement")]
    [SerializeField] float speed = 1.5f;
    [SerializeField] float detectionRadius = 9f;
    [SerializeField] float stopDistance = 0.2f;     

    [Header("Catch")]
    [SerializeField] float catchRadius = 0.7f;        
    [SerializeField] float catchCooldown = 1.25f;   
    [SerializeField] LayerMask obstacleMask = 0;   
    [SerializeField] bool requireLineOfSight = false;  

    [Header("Feedback (optional)")]
    [SerializeField] UIMessage messageUI; 
    [SerializeField] string caughtMessage = "Caught! Switches reset.";

    Rigidbody2D rb;
    Vector2 startPos;
    float cooldown;
    SpriteRenderer sr;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        rb.gravityScale = 0f;
        rb.freezeRotation = true;
        rb.collisionDetectionMode = CollisionDetectionMode2D.Continuous;
        rb.interpolation = RigidbodyInterpolation2D.Interpolate;

        startPos = transform.position;

        if (player == null)
        {
            var pc = FindObjectOfType<PlayerController>();
            if (pc) player = pc.transform;
        }
        if (messageUI == null) messageUI = FindObjectOfType<UIMessage>();
        sr = GetComponentInChildren<SpriteRenderer>();
    }

    void FixedUpdate()
    {
        if (player == null) return;
        if (cooldown > 0f) { cooldown -= Time.fixedDeltaTime; return; }

        Vector2 myPos = rb.position;
        Vector2 targetPos = player.position;
        Vector2 toPlayer = targetPos - myPos;
        float dist = toPlayer.magnitude;

        if (sr != null && Mathf.Abs(toPlayer.x) > 0.02f) sr.flipX = (toPlayer.x < 0f);

        if (dist <= catchRadius)
        {
            HandleCatch();
            return;
        }

        if (dist <= detectionRadius && HasLineOfSight(myPos, targetPos))
        {
            if (dist > stopDistance)
            {
                Vector2 dir = toPlayer / Mathf.Max(dist, 0.0001f);
                Vector2 next = myPos + dir * speed * Time.fixedDeltaTime;
                rb.MovePosition(next);
            }
        }
    }

    bool HasLineOfSight(Vector2 from, Vector2 to)
    {
        if (!requireLineOfSight || obstacleMask == 0) return true;
        var hit = Physics2D.Raycast(from, (to - from).normalized, Vector2.Distance(from, to), obstacleMask);
        return hit.collider == null;
    }

    void HandleCatch()
    {
        // 1) Reset all switches
        FindObjectOfType<SwitchManager>()?.ResetAll();

        // 2) Message
        if (messageUI != null && !string.IsNullOrEmpty(caughtMessage))
            messageUI.Show(caughtMessage, 1.2f);

        if (playerRespawnPoint != null && player != null)
            player.position = playerRespawnPoint.position;

        transform.position = startPos;

        cooldown = catchCooldown;
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = new Color(1f, 0.2f, 0.2f, 0.25f);
        Gizmos.DrawWireSphere(transform.position, detectionRadius);

        Gizmos.color = new Color(1f, 0f, 0f, 0.35f);
        Gizmos.DrawWireSphere(transform.position, catchRadius);
    }
}