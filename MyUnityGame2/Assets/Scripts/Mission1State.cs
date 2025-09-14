using UnityEngine;

public class Mission1State : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private UIMessage messageUI; // Drag your message UI prefab here

    public bool PowerOn { get; private set; }
    public bool DoorTried { get; private set; }

    public void SetPowerOn()
    {
        PowerOn = true;
    }

    public void MarkDoorTried()
    {
        DoorTried = true;
    }

    public bool CanSearchClues
    {
        get
        {
            if (!PowerOn)
            {
                // Lights not fixed yet → show warning
                if (messageUI != null)
                    messageUI.Show("It's too dark to search!");
                return false;
            }

            if (!DoorTried)
            {
                // Door not tried yet → optional block
                if (messageUI != null)
                    messageUI.Show("");
                return false;
            }

            return true;
        }
    }
}