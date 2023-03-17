using UnityEngine;

public class Provoke : Order
{
    public Provoke() {}
    public string Name() { return "Provoke"; }
    public void PlayOrderSound(UnitSound unitSound, AudioSource system) {}

    public bool ControllUnit(Unit unit)
    {
        unit.MoveTo(unit.CurPosition);
        unit.Roar();
        return true;
    }
}
