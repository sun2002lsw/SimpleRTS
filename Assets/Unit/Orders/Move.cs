using UnityEngine;

public class Move : Order
{
    Vector3 destination;

    public Move(Vector3 destination)
    {
        this.destination = destination;
    }

    public string Name()
    {
        return "Move";
    }

    public bool ControllUnit(Unit unit)
    {
        return unit.MoveToDestination(destination);
    }
}
