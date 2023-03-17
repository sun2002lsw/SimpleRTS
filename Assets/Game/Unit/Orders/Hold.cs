using UnityEngine;

public class Hold : Order
{
    public string Name()
    {
        return "Hold";
    }

    public void PlayOrderSound(UnitSound unitSound, AudioSource system)
    {
        unitSound.PlayHoldVoiceSound(system);
    }

    public bool ControllUnit(Unit unit)
    {
        unit.DefendPosition();
        return false;
    }
}
