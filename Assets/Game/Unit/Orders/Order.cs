using IsOrderComplete = System.Boolean;

public interface Order
{
    public string Name();
    public IsOrderComplete ControllUnit(Unit unit);
}
