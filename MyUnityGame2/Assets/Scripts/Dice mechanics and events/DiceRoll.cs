using UnityEngine;

public class DiceRoll : MonoBehaviour
{
    public Sprite[] diceFaces; // assign your 6 sprites in Inspector
    private SpriteRenderer sr;
    private bool isRolling = false;

    void Start()
    {
        sr = GetComponent<SpriteRenderer>();
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            RollDice();
        }
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
