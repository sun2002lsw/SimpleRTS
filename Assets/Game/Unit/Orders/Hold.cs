using UnityEngine;

public class Hold : Order
{
    public string Name()
    {
        return "Hold";
    }

    public void PlayOrderSound(UnitSound unitSound)
    {
        unitSound.PlayHoldVoiceSound();
    }

    public bool ControllUnit(Unit unit)
    {
        unit.DefendPosition();
        return false;
    }
}
