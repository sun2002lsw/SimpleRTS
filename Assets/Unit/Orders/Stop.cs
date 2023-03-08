using UnityEngine;

public class Stop : Order
{
    Vector3 originPosition;

    public Stop(Vector3 originPosition)
    {
        this.originPosition = originPosition;
    }

    public bool ControllUnit(Unit unit)
    {
        return true;
    }
}
