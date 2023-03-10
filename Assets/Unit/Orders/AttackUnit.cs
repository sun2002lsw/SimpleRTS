public class AttackUnit : Order
{
    Unit target;

    public AttackUnit(Unit target)
    {
        this.target = target;
    }

    public string Name()
    {
        return "AttackUnit";
    }

    public bool ControllUnit(Unit unit)
    {
        return true;
    }
}
