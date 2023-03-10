using UnityEngine;

public class Stop : Order
{
    Vector3 originPosition;

    public Stop(Vector3 originPosition)
    {
        this.originPosition = originPosition;
    }

    public string Name()
    {
        return "Stop";
    }

    public bool ControllUnit(Unit unit)
    {
        return true;
    }
}
