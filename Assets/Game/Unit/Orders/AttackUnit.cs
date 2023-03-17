public class AttackUnit : Order
{
    private Unit target;

    public AttackUnit(Unit target)
    {
        this.target = target;
    }

    public string Name()
    {
        return "AttackUnit";
    }

    public void PlayOrderSound(UnitSound unitSound)
    {
        unitSound.PlayAttackVoiceSound();
    }

    public bool ControllUnit(Unit unit)
    {
        return unit.AttackUnit(target);
    }
}
