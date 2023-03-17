using UnityEngine;

public class Hold : Order
{
    public string Name()
    {
        return "Hold";
    }

    public bool ControllUnit(Unit unit)
    {
        unit.DefendPosition();
        return false;
    }
}
