using UnityEngine;
using TMPro;

public class KeypadUI : MonoBehaviour
{
    [SerializeField] PasscodeManager passcode;
    [SerializeField] LockedDoor door;
    [SerializeField] TextMeshProUGUI display;
    [SerializeField] UIMessage messageUI;

    string input = "";

    void OnEnable() { input = ""; UpdateDisplay(); }

    public void PressDigit(int d)
    {
        if (input.Length >= passcode.CodeLength) return;
        input += d.ToString();
        UpdateDisplay();
    }

    public void ClearAll() { input = ""; UpdateDisplay(); }

    public void Submit()
    {
        if (input.Length != passcode.CodeLength)
        { messageUI?.Show("Enter full code."); return; }

        if (input == passcode.GetCode())
        { messageUI?.Show("Access granted."); door.Unlock(); gameObject.SetActive(false); }
        else
        { messageUI?.Show("Access denied."); input = ""; UpdateDisplay(); }
    }

    public void Close()
    {
        gameObject.SetActive(false);
    }


    void UpdateDisplay()
    {
        int n = passcode.CodeLength;
        char[] buf = new char[n];
        for (int i = 0; i < n; i++) buf[i] = i < input.Length ? input[i] : '•';
        display.text = new string(buf);
    }

    void Update()
    {
        if (!gameObject.activeInHierarchy) return;

        for (var k = KeyCode.Alpha0; k <= KeyCode.Alpha9; k++)
            if (Input.GetKeyDown(k))
                PressDigit((int)k - (int)KeyCode.Alpha0);

        for (var k = KeyCode.Keypad0; k <= KeyCode.Keypad9; k++)
            if (Input.GetKeyDown(k))
                PressDigit((int)k - (int)KeyCode.Keypad0);

        if (Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.KeypadEnter))
            Submit();

        if (Input.GetKeyDown(KeyCode.Backspace))
            ClearAll();

        if (Input.GetKeyDown(KeyCode.Escape))
            Close();
    }
}