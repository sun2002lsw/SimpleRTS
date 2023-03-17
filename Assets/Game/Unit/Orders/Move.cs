using UnityEngine;

public class Move : Order
{
    private Vector3 destination;

    public Move(Vector3 destination)
    {
        this.destination = destination;
    }

    public string Name()
    {
        return "Move";
    }

    public void PlayOrderSound(UnitSound unitSound)
    {
        unitSound.PlayMoveVoiceSound();
    }

    public bool ControllUnit(Unit unit)
    {
        return unit.MoveTo(destination);
    }
}
