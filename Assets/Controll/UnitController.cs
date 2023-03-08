using System.Collections.Generic;
using UnityEngine;

public class UnitController : MonoBehaviour
{
    List<Unit> allyUnits = new List<Unit>();
    GameObject decoy;

    void Start()
    {
        foreach(var gameObject in GameObject.FindGameObjectsWithTag("ally"))
            allyUnits.Add(gameObject.GetComponent<Unit>());

        decoy = GameObject.FindGameObjectWithTag("enemy");
    }

    void Update()
    {
        Vector3 destination = decoy.transform.position;

        if (Input.GetKey(KeyCode.Mouse1))
            foreach (var unit in allyUnits)
            {
                Vector3 unitDestination = refineDestinationForUnit(unit, destination);
                unit.SetOrder(new Move(unitDestination));
            }
    }

    Vector3 refineDestinationForUnit(Unit unit, Vector3 destination)
    {
        return new Vector3(destination.x, unit.transform.position.y, destination.z);
    }
}
