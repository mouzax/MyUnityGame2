using UnityEngine;
using System.Text;

public class PasscodeManager : MonoBehaviour
{
    [Header("Code Settings")]
    [SerializeField] int codeLength = 4;
    [SerializeField] bool randomizeAtStart = false;
    [SerializeField] string fixedCode = "3197";
    [SerializeField] TMPro.TextMeshProUGUI codeHUD;
    [SerializeField] bool showOnlyWhenComplete = true;

    [Header("UI (optional)")]
    [SerializeField] UIMessage messageUI;

    string code;
    bool[] revealed;

    void Awake()
    {
        revealed = new bool[codeLength];

        if (randomizeAtStart)
        {
            var sb = new StringBuilder(codeLength);
            for (int i = 0; i < codeLength; i++)
                sb.Append(Random.Range(0, 10));
            code = sb.ToString();
        }
        else
        {
            if (string.IsNullOrEmpty(fixedCode) || fixedCode.Length != codeLength)
                fixedCode = "0000".Substring(0, codeLength);
            code = fixedCode;
        }

        Debug.Log($"[PasscodeManager] Code = {code} (hidden from player)");
        UpdateHUD();
    }

    public int CodeLength => codeLength;

    public char GetDigit(int index) => code[index];

    public void RevealDigit(int index)
    {
        if (index < 0 || index >= codeLength) return;
        if (revealed[index]) return;
        revealed[index] = true;

        if (messageUI)
        {
            var pretty = IndexToWord(index);
            messageUI.Show($"{Cap(pretty)} digit is {code[index]}");
        }
        UpdateHUD();
    }

    public bool IsRevealed(int index) => (index >= 0 && index < codeLength) && revealed[index];

    public bool AllRevealed()
    {
        for (int i = 0; i < codeLength; i++) if (!revealed[i]) return false;
        return true;
    }

    public string GetKnownPattern(char unknownChar = '•')
    {
        var sb = new StringBuilder(codeLength);
        for (int i = 0; i < codeLength; i++)
            sb.Append(revealed[i] ? code[i] : unknownChar);
        return sb.ToString();
    }

    public string GetCode() => code;

    string IndexToWord(int i) => i switch { 0 => "first", 1 => "second", 2 => "third", 3 => "fourth", _ => $"{i + 1}th" };
    string Cap(string s) => string.IsNullOrEmpty(s) ? s : char.ToUpper(s[0]) + s.Substring(1);

    void UpdateHUD()
    {
        if (!codeHUD) return;

        if (showOnlyWhenComplete && !AllRevealed())
        {
            codeHUD.text = "";
            return;
        }

        codeHUD.text = $"Code: {GetKnownPattern('•')}";
    }
}