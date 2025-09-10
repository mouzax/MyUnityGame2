using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;

public class MazeUI : MonoBehaviour
{
    [Header("UI Refs")]
    [SerializeField] RectTransform gridParent; 
    [SerializeField] Image cellPrefab;
    [SerializeField] Button backButton;
    [SerializeField] TextMeshProUGUI titleText;

    [Header("Messages & Game Hooks")]
    [SerializeField] UIMessage messageUI;
    [SerializeField] PasscodeManager passcode;
    [SerializeField] int[] digitsToReveal = { 2 };
    [SerializeField] ComputerTerminal terminal;

    [Header("Colors (alpha must be 1)")]
    [SerializeField] Color wallColor   = new Color(0.20f, 0.20f, 0.20f, 1f);
    [SerializeField] Color floorColor  = new Color(0.25f, 0.45f, 0.30f, 1f);
    [SerializeField] Color startColor  = new Color(0.20f, 0.60f, 0.90f, 1f);
    [SerializeField] Color exitColor   = new Color(0.90f, 0.30f, 0.25f, 1f);
    [SerializeField] Color playerColor = Color.yellow;

    int[,] maze =
    {
        {1,1,1,1,1,1,1,1,1},
        {1,0,0,0,1,0,0,0,1},
        {1,0,1,0,1,0,1,0,1},
        {1,0,1,0,0,0,1,0,1},
        {1,0,1,1,1,0,1,0,1},
        {1,0,0,0,1,0,0,0,1},
        {1,1,1,0,1,1,1,0,1},
        {1,0,0,0,0,0,1,0,1},
        {1,1,1,1,1,1,1,1,1}
    };

    readonly int cols = 9;
    readonly int rows = 9;
    Vector2Int start = new Vector2Int(1, 1);
    Vector2Int exit  = new Vector2Int(7, 7);
    Vector2Int player;

    Image[,] cells;
    bool built = false;

    void Awake()
    {
        if (backButton) backButton.onClick.AddListener(Close);
        if (terminal == null) terminal = FindObjectOfType<ComputerTerminal>();
    }

    void OnEnable()
    {
        BuildOnce();
        ResetRun();
        DrawAll();
    }

    void Update()
    {
        if (!gameObject.activeInHierarchy) return;

        if (Input.GetKeyDown(KeyCode.Escape)) { Close(); return; }

        Vector2Int dir = Vector2Int.zero;
        if (Input.GetKeyDown(KeyCode.W) || Input.GetKeyDown(KeyCode.UpArrow))        dir = new Vector2Int(0, -1);
        else if (Input.GetKeyDown(KeyCode.S) || Input.GetKeyDown(KeyCode.DownArrow)) dir = new Vector2Int(0,  1);
        else if (Input.GetKeyDown(KeyCode.A) || Input.GetKeyDown(KeyCode.LeftArrow)) dir = Vector2Int.left;
        else if (Input.GetKeyDown(KeyCode.D) || Input.GetKeyDown(KeyCode.RightArrow))dir = Vector2Int.right;

        if (dir != Vector2Int.zero) TryMove(dir);
    }

    void BuildOnce()
    {
        if (built) return;

        if (gridParent && (gridParent.rect.width < 10f || gridParent.rect.height < 10f))
            gridParent.sizeDelta = new Vector2(360f, 360f);

        cells = new Image[rows, cols];

        for (int y = 0; y < rows; y++)
        {
            for (int x = 0; x < cols; x++)
            {
                Image img = Instantiate(cellPrefab, gridParent);
                img.raycastTarget = false;
                cells[y, x] = img;
            }
        }

        built = true;
    }

    void ResetRun()
    {
        player = start;
    }

    void TryMove(Vector2Int dir)
    {
        Vector2Int next = player + dir;
        if (next.x < 0 || next.x >= cols || next.y < 0 || next.y >= rows) return;
        if (maze[next.y, next.x] == 1) return;

        player = next;
        DrawAll();

        if (player == exit)
        {
            OnWin();
        }
    }

    void DrawAll()
    {
        for (int y = 0; y < rows; y++)
        {
            for (int x = 0; x < cols; x++)
            {
                Image img = cells[y, x];
                if (!img) continue;

                Color c = (maze[y, x] == 1) ? wallColor : floorColor;
                if (x == start.x && y == start.y) c = startColor;
                if (x == exit.x  && y == exit.y)  c = exitColor;

                img.color = c;
            }
        }

        cells[player.y, player.x].color = playerColor;
    }

    void OnWin()
    {
        if (passcode != null && digitsToReveal != null)
    {
        foreach (var idx in digitsToReveal)
        {
            if (idx >= 0 && idx < passcode.CodeLength && !passcode.IsRevealed(idx))
                passcode.RevealDigit(idx);
        }
    }
        if (messageUI != null && passcode != null)
        {
            messageUI.Show("Hack successful. Code updated.", 1.2f);

            string pattern = passcode.GetKnownPattern('•');

            messageUI.StartCoroutine(ShowAfterDelay(messageUI, $"Known: {pattern}", 1.2f));
        }

        if (terminal != null) terminal.MarkCompleted();

        Close();
    }

    System.Collections.IEnumerator ShowAfterDelay(UIMessage ui, string text, float delay)
    {
        yield return new WaitForSeconds(delay);
        if (ui != null) ui.Show(text, 2f);
    }

    IEnumerator ShowKnownPatternAfterDelay(string pattern, float delay)
    {
        yield return new WaitForSeconds(delay);
        messageUI?.Show($"Known: {pattern}", 2f);
    }

    public void Close()
    {
        gameObject.SetActive(false);
    }
}