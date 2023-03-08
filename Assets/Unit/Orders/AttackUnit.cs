public class AttackUnit : Order
{
    Unit target;

    public AttackUnit(Unit target)
    {
        this.target = target;
    }

    public bool ControllUnit(Unit unit)
    {
        return true;
    }
}
