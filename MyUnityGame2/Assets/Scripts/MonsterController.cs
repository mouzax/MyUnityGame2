using UnityEngine;
using System;

[RequireComponent(typeof(Rigidbody2D), typeof(Collider2D))]
public class MonsterController : MonoBehaviour
{
    public static event Action OnPlayerCaught; // 🔑 Event fired when monster touches player

    [Header("Chase Settings")]
    [SerializeField] float moveSpeed = 3.5f;
    [SerializeField] float chaseRange = 20f;

    [Header("Spawn Settings")]
    [SerializeField] Vector2 spawnPosition = new Vector2(-9, 4);

    private Transform player;
    private Rigidbody2D rb;
    private bool isActive = false;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        rb.gravityScale = 0f;
        rb.freezeRotation = true;

        // find player by tag
        GameObject p = GameObject.FindGameObjectWithTag("Player");
        if (p != null) player = p.transform;

        // ensure collider is trigger
        Collider2D col = GetComponent<Collider2D>();
        if (col != null) col.isTrigger = true;
    }

    void Update()
    {
        if (!isActive || player == null) return;

        float dist = Vector2.Distance(transform.position, player.position);
        if (dist > chaseRange) return;

        Vector2 dir = (player.position - transform.position).normalized;
        rb.MovePosition(rb.position + dir * moveSpeed * Time.deltaTime);
    }

    public void Spawn(Vector2 position)
    {
        transform.position = position;
        gameObject.SetActive(true);
        isActive = true;
    }

    public void Despawn()
    {
        isActive = false;
        rb.linearVelocity = Vector2.zero;
        gameObject.SetActive(false);
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (!isActive) return;

        if (other.CompareTag("Player"))
        {
            Debug.Log("[MonsterController] Player caught!");
            OnPlayerCaught?.Invoke();

            // Optionally stop moving immediately
            rb.linearVelocity = Vector2.zero;
        }
    }
}