using IsOrderComplete = System.Boolean;

public interface Order
{
    public IsOrderComplete ControllUnit(Unit unit);
}
