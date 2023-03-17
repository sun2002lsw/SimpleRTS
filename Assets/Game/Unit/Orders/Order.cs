using UnityEngine;
using IsOrderComplete = System.Boolean;

public interface Order
{
    public string Name();
    public void PlayOrderSound(UnitSound unitSound, AudioSource system);
    public IsOrderComplete ControllUnit(Unit unit);
}
