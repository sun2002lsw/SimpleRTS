using UnityEngine;

public class AttackGround : Order
{
    private Vector3 destination;

    public AttackGround(Vector3 destination)
    {
        this.destination = destination;
    }

    public string Name()
    {
        return "AttackGround";
    }

    public void PlayOrderSound(UnitSound unitSound)
    {
        unitSound.PlayAttackVoiceSound();
    }

    public bool ControllUnit(Unit unit)
    {
        if (unit.DetectedEnemy == null)
            return unit.MoveTo(destination);
        else
            return unit.AttackUnit(unit.DetectedEnemy);
    }
}
