using UnityEngine;
using TMPro;

public class UIMessage : MonoBehaviour
{
    [SerializeField] float visibleTime = 2f;

    TextMeshProUGUI text;
    float timer;

    void Awake()
    {
        text = GetComponent<TextMeshProUGUI>();
        text.text = "";
    }

    void Update()
    {
        if (timer > 0f)
        {
            timer -= Time.deltaTime;
            if (timer <= 0f)
            {
                text.text = "";
            }
        }
    }

    public void Show(string msg, float duration = -1f)
    {
        if (duration <= 0f) duration = visibleTime;
        text.text = msg;
        timer = duration;
    }
}