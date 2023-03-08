using UnityEngine;

public class Move : Order
{
    Vector3 destination;

    public Move(Vector3 destination)
    {
        this.destination = destination;
    }

    public bool ControllUnit(Unit unit)
    {
        if (unit.IsArrivedAt(destination))
            return true;

        if (unit.RotateTo(destination)) 
            if (unit.MoveTo(destination))
                return true;

        return false;
    }
}
