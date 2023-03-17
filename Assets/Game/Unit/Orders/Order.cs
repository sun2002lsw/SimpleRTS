using IsOrderComplete = System.Boolean;

public interface Order
{
    public string Name();
    public void PlayOrderSound(UnitSound unitSound);
    public IsOrderComplete ControllUnit(Unit unit);
}
