using UnityEngine;

public class Stop : Order
{
    private Vector3 originPosition;

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
        if (unit.DetectedEnemy == null)
            return true;

        // todo æÓ≈√∂•
        return unit.RotateTo(unit.DetectedEnemy.Position);
    }
}
