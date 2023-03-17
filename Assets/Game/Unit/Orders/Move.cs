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

    public void PlayOrderSound(UnitSound unitSound, AudioSource system)
    {
        unitSound.PlayMoveVoiceSound(system);
    }

    public bool ControllUnit(Unit unit)
    {
        return unit.MoveTo(destination);
    }
}
