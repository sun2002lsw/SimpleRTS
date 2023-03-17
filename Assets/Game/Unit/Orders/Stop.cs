using System.Linq.Expressions;
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
            return unit.MoveTo(originPosition);
        else
            return unit.AttackUnit(unit.DetectedEnemy);
    }
}
