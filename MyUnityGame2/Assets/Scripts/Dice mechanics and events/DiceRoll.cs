using UnityEngine;
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
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
            RollDice();
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

        int resultIndex = Random.Range(0, diceFaces.Length);
        sr.sprite = diceFaces[resultIndex];
        int diceNumber = resultIndex + 1;

        Debug.Log("Dice result: " + diceNumber);

        if (diceEvents != null && resultIndex < diceEvents.Length && diceEvents[resultIndex] != null)
            diceEvents[resultIndex].Invoke();

        isRolling = false;
    }
}