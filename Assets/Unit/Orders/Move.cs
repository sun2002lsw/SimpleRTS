using UnityEngine;

public class Move : Order
{
    Transform destination;

    public Move(Transform destination)
    {
        this.destination = destination;
    }

    public bool ControllUnit(Unit unit)
    {
        return true;
    }
}
