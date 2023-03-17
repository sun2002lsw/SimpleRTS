using UnityEngine;

public class Stop : Order
{
    private Vector3 originPosition;

    public Stop(Vector3 originPosition)
    {
        this.originPosition = originPosition;
    }

    public string Name()
    {
        return "Stop";
    }

    public void PlayOrderSound(UnitSound unitSound, AudioSource system)
    {
        unitSound.PlayStopVoiceSound(system);
    }

    public bool ControllUnit(Unit unit)
    {
        if (unit.DetectedEnemy == null)
            return unit.MoveTo(originPosition);

        unit.AttackUnit(unit.DetectedEnemy);
        return false;
    }
}
