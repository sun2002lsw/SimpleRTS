using UnityEngine;

public class Hold : Order
{
    public string Name()
    {
        return "Hold";
    }

    public bool ControllUnit(Unit unit)
    {
        return true;
    }
}
