using UnityEngine;
using System;

public enum LabThing { Microscope, Puddle, Paper, Flask, Book}

[RequireComponent(typeof(Collider2D))]
public class LabInteractable : MonoBehaviour
{
    public static event Action<LabInteractable> OnActivatedGlobal;

    [Header("Type / Interaction")]
    [SerializeField] LabThing type = LabThing.Microscope;
    [SerializeField] KeyCode interactKey = KeyCode.Space;
    [SerializeField] bool oneTime = true;

    [Header("Visuals")]
    [SerializeField] SpriteRenderer mainRenderer;
    [SerializeField] Sprite offSprite;
    [SerializeField] Sprite onSprite;
    [SerializeField] Color offColor = new(1f, 0.4f, 0.4f, 1f);
    [SerializeField] Color onColor  = new(0.4f, 1f, 0.4f, 1f);
    [SerializeField] GameObject extraFXOn;
    [SerializeField] AudioClip sfxActivate;

    [Header("Prompt (optional)")]
    [SerializeField] UIMessage messageUI;
    [SerializeField] string activatedMsg = "System restored";

    bool playerIn;
    bool activated;
    AudioSource audioSrc;

    public bool Activated => activated;
    public LabThing Type => type;

    void Reset()
    {
        var col = GetComponent<Collider2D>();
        col.isTrigger = true;
        if (!mainRenderer) mainRenderer = GetComponentInChildren<SpriteRenderer>();
    }

    void Awake()
    {
        if (!mainRenderer) mainRenderer = GetComponentInChildren<SpriteRenderer>();
        audioSrc = GetComponent<AudioSource>();
        ApplyVisual(offState:true);
    }

    void OnTriggerEnter2D(Collider2D other) { if (other.CompareTag("Player")) playerIn = true; }
    void OnTriggerExit2D (Collider2D other) { if (other.CompareTag("Player")) playerIn = false; }

    void Update()
    {
        if (!playerIn || activated && oneTime) return;
        if (Input.GetKeyDown(interactKey)) Activate();
    }

    public void Activate()
    {
        if (activated && oneTime) return;
        activated = true;

        ApplyVisual(offState:false);

        if (sfxActivate)
        {
            if (!audioSrc) audioSrc = gameObject.AddComponent<AudioSource>();
            audioSrc.PlayOneShot(sfxActivate);
        }

        if (messageUI && !string.IsNullOrEmpty(activatedMsg))
            messageUI.Show($"{NiceName(type)}: {activatedMsg}", 1.2f);

        OnActivatedGlobal?.Invoke(this);
    }

    void ApplyVisual(bool offState)
    {
        if (!mainRenderer) return;

        if (offSprite && onSprite)
            mainRenderer.sprite = offState ? offSprite : onSprite;
        else
            mainRenderer.color  = offState ? offColor  : onColor;

        if (extraFXOn) extraFXOn.SetActive(!offState);
    }

    string NiceName(LabThing t) => t switch
    {
        LabThing.Microscope => "Microscope",
        LabThing.Puddle    => "Puddle",
        LabThing.Paper   => "Workstation",
        LabThing.Flask      => "Chemistry Unit",
        LabThing.Book      => "Book",
        _ => "System"
    };
}