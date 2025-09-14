using UnityEngine;
using System;

public enum LabThing { Microscope, GooTank, Computer, Flask, Book, Puddle, Paper }

[RequireComponent(typeof(Collider2D))]
public class LabInteractable : MonoBehaviour
{
    public static event Action<LabInteractable> OnActivatedGlobal;

    [Header("Type / Interaction")]
    [SerializeField] LabThing type = LabThing.Microscope;
    [SerializeField] KeyCode interactKey = KeyCode.Space;
    [SerializeField] bool oneTime = true;

    [Header("Visuals (Backlight system)")]
    [SerializeField] GameObject backlight;   // Light or glow sprite object (child under this item)
    [SerializeField] GameObject extraFXOn;   // Optional: particles, sparks, bubbles
    [SerializeField] AudioClip sfxActivate;  // Optional

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
    }

    void Awake()
    {
        audioSrc = GetComponent<AudioSource>();
        ApplyVisual(offState: true);
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player")) playerIn = true;
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player")) playerIn = false;
    }

    void Update()
    {
        if (!playerIn || (activated && oneTime)) return;

        if (Input.GetKeyDown(interactKey))
            Activate();
    }

    public void Activate()
    {
        if (activated && oneTime) return;
        activated = true;

        ApplyVisual(offState: false);

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
        if (backlight) backlight.SetActive(!offState); // Glow ON when available, OFF when activated
        if (extraFXOn) extraFXOn.SetActive(!offState); // Extra FX only when ON
    }

    string NiceName(LabThing t) => t switch
    {
        LabThing.Microscope => "Microscope",
        LabThing.GooTank    => "Containment Tank",
        LabThing.Computer   => "Workstation",
        LabThing.Flask      => "Chemistry Unit",
        LabThing.Book       => "Research Book",
        LabThing.Puddle     => "Spilled Chemicals",
        LabThing.Paper      => "Lab Notes",
        _ => "System"
    };
}