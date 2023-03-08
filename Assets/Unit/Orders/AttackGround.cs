using UnityEngine;

public class AttackGround : Order
{
    Transform destination;

    public AttackGround(Transform destination)
    {
        this.destination = destination;
    }

    public bool ControllUnit(Unit unit)
    {
        return true;
    }
}
