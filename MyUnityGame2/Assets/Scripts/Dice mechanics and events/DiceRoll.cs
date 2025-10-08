using UnityEngine;
<<<<<<< Updated upstream
using UnityEngine.Events;

public class DiceRoll : MonoBehaviour
{
    [Header("Dice visuals")]
    public Sprite[] diceFaces;
    private SpriteRenderer sr;
    private bool isRolling = false;

    [Header("What happens for each dice result (1..6)")]
    [Tooltip("Size must be 6. Element 0 = when result is 1, Element 5 = when result is 6.")]
    public UnityEvent[] diceEvents = new UnityEvent[6];

    void Start()
    {
        sr = GetComponent<SpriteRenderer>();
        if (diceEvents == null || diceEvents.Length != 6)
            diceEvents = new UnityEvent[6];
=======

public class DiceRoll : MonoBehaviour
{
    public Sprite[] diceFaces; // assign your 6 sprites in Inspector
    private SpriteRenderer sr;
    private bool isRolling = false;

    void Start()
    {
        sr = GetComponent<SpriteRenderer>();
>>>>>>> Stashed changes
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
<<<<<<< Updated upstream
            RollDice();
=======
        {
            RollDice();
        }
>>>>>>> Stashed changes
    }

    public void RollDice()
    {
        if (!isRolling)
            StartCoroutine(RollAnimation());
    }

    private System.Collections.IEnumerator RollAnimation()
    {
        isRolling = true;

        float rollDuration = 1f;
        float elapsed = 0f;

        while (elapsed < rollDuration)
        {
            sr.sprite = diceFaces[Random.Range(0, diceFaces.Length)];
            elapsed += 0.1f;
            yield return new WaitForSeconds(0.1f);
        }

<<<<<<< Updated upstream
        int resultIndex = Random.Range(0, diceFaces.Length);
        sr.sprite = diceFaces[resultIndex];
        int diceNumber = resultIndex + 1;

        Debug.Log("Dice result: " + diceNumber);

        if (diceEvents != null && resultIndex < diceEvents.Length && diceEvents[resultIndex] != null)
            diceEvents[resultIndex].Invoke();

        isRolling = false;
    }
}
=======
        int result = Random.Range(0, diceFaces.Length); // 0–5
        sr.sprite = diceFaces[result];
        Debug.Log("Dice result: " + (result + 1));

        // 👉 Call the event for this dice number
        TriggerDiceEvent(result + 1);

        isRolling = false;
    }

    private void TriggerDiceEvent(int diceNumber)
    {
        switch (diceNumber)
        {
            case 1: FindObjectOfType<LightsOut>().StartEvent(); break;
            case 2: FindObjectOfType<PadlockDigits>().StartEvent(); break;
            case 3: FindObjectOfType<ShakingRoom>().StartEvent(); break;
            case 4: FindObjectOfType<LaserTrapwire>().StartEvent(); break;
            case 5: FindObjectOfType<ElectricDoor>().StartEvent(); break;
            case 6: FindObjectOfType<GeneratorEvent>().StartEvent(); break;
        }
    }
}
>>>>>>> Stashed changes
