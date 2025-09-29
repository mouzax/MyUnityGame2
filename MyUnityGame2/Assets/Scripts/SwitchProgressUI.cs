using UnityEngine;
using TMPro;

public class SwitchProgressUI : MonoBehaviour
{
    [Header("UI")]
    [SerializeField] TextMeshProUGUI text;

    [Header("Display")]
    [SerializeField] char checkChar = '✓';
    [SerializeField] char dotChar = '.';
    [SerializeField] string separator = " ";

    [Header("Refresh")]
    [SerializeField] float refreshEvery = 0.2f;

    LabSwitch[] switches;
    float timer;

    void Awake()
    {
        if (text == null) text = GetComponent<TextMeshProUGUI>();
    }

    void Start()
    {
        switches = FindObjectsOfType<LabSwitch>(includeInactive: false);
        Refresh();
    }

    void Update()
    {
        timer -= Time.deltaTime;
        if (timer <= 0f)
        {
            timer = refreshEvery;
            Refresh();
        }
    }

    public void Refresh()
    {
        if (text == null) return;
        if (switches == null) switches = FindObjectsOfType<LabSwitch>(includeInactive: false);

        int total = switches.Length;
        int on = 0;
        for (int i = 0; i < total; i++) if (switches[i].IsOn) on++;

        System.Text.StringBuilder sb = new System.Text.StringBuilder(total * 2);
        for (int i = 0; i < total; i++)
        {
            sb.Append(i < on ? checkChar : dotChar);
            if (i < total - 1) sb.Append(separator);
        }

        text.text = sb.ToString();
    }
    
    void OnEnable()  { LabSwitch.OnActivated += _ => Refresh(); SwitchManager.OnSwitchesReset += Refresh; }
    void OnDisable() { LabSwitch.OnActivated -= _ => Refresh(); SwitchManager.OnSwitchesReset -= Refresh; }
}