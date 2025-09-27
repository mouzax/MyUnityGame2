using UnityEngine;
using System;

public enum LabSwitchType { Microscope, Flask, Book, Puddle, Paper }

[RequireComponent(typeof(SpriteRenderer))]
[RequireComponent(typeof(Collider2D))]
public class LabSwitch : MonoBehaviour
{
    [Header("Setup")]
    public LabSwitchType type;
    public bool oneTime = true;

    [Header("Colors (no lights)")]
    public Color offColor = Color.white;
    public Color onColor  = Color.green;

    [Header("UI Prompt")]
    [SerializeField] UIMessage messageUI; 
    [SerializeField] string inspectPrompt = "Press SPACE to inspect";
    [SerializeField] string activatedMsg  = ""; 

    [Header("State (read-only)")]
    [SerializeField] private bool isOn = false;
    public bool IsOn => isOn;

    private SpriteRenderer sr;
    private Collider2D col;

    public static event Action<LabSwitch> OnActivated;

    void Awake()
    {
        sr  = GetComponent<SpriteRenderer>();
        col = GetComponent<Collider2D>();

        col.isTrigger = true;
        sr.color = isOn ? onColor : offColor;

        if (messageUI == null) messageUI = FindObjectOfType<UIMessage>();
    }

    void OnTriggerStay2D(Collider2D other)
    {
        if (isOn) return;
        if (!IsPlayer(other)) return;

        if (messageUI != null)
        {
            messageUI.Show(inspectPrompt, 0.2f);
        }
    }

    void OnTriggerExit2D(Collider2D other)
    {
    }

    bool IsPlayer(Collider2D other)
    {
        return other.GetComponent<PlayerController>() != null;
    }

    public void Activate()
    {
        if (isOn && oneTime) return;

        isOn = true;
        sr.color = onColor;

        if (!string.IsNullOrEmpty(activatedMsg) && messageUI != null)
            messageUI.Show(activatedMsg, 0.5f);

        OnActivated?.Invoke(this);
    }

    public void ResetSwitch()
    {
        isOn = false;
        sr.color = offColor;
    }

#if UNITY_EDITOR
    void OnValidate()
    {
        var r = GetComponent<SpriteRenderer>();
        if (r != null) r.color = isOn ? onColor : offColor;

        var c = GetComponent<Collider2D>();
        if (c != null) c.isTrigger = true;
    }
#endif
}