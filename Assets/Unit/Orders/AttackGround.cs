using UnityEngine;

public class AttackGround : Order
{
    Vector3 destination;

    public AttackGround(Vector3 destination)
    {
        this.destination = destination;
    }

    public string Name()
    {
        return "AttackGround";
    }

    public bool ControllUnit(Unit unit)
    {
        return true;
    }
}
