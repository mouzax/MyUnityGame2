using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
public class YSort : MonoBehaviour
{
    [SerializeField] int orderOffset = 0; // tweak per object if needed
    SpriteRenderer sr;

    void Awake() { sr = GetComponent<SpriteRenderer>(); }

    void LateUpdate()
    {
        // lower Y => larger sortingOrder (on top)
        sr.sortingOrder = orderOffset - Mathf.RoundToInt(transform.position.y * 100f);
    }
}
