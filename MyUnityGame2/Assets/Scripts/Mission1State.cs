using UnityEngine;

public class Mission1State : MonoBehaviour
{
    public bool PowerOn { get; private set; }
    public bool DoorTried { get; private set; }

    public void SetPowerOn()    => PowerOn = true;
    public void MarkDoorTried() => DoorTried = true;

    public bool CanSearchClues => PowerOn && DoorTried;
}