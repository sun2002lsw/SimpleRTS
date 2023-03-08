using UnityEngine;

public class Stop : Order
{
    Transform originPosition;

    public Stop(Transform originPosition)
    {
        this.originPosition = originPosition;
    }

    public bool ControllUnit(Unit unit)
    {
        return true;
    }
}
