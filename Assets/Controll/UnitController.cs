using System.Collections.Generic;
using UnityEngine;

public class UnitController : MonoBehaviour
{
    Camera mainCamera;

    List<Unit> allyUnits = new List<Unit>();

    private void Awake()
    {
        mainCamera = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<Camera>();
    }

    void Start()
    {
        foreach(var gameObject in GameObject.FindGameObjectsWithTag("ally"))
            allyUnits.Add(gameObject.GetComponent<Unit>());
    }

    void Update()
    {
        if (!Input.GetKey(KeyCode.Mouse1))
            return;

        Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);
        RaycastHit raycastHit;
        if (Physics.Raycast(ray, out raycastHit))
            foreach (var unit in allyUnits)
            {
                Vector3 unitDestination = refineDestinationForUnit(unit, raycastHit.point);
                unit.SetOrder(new Move(unitDestination));
            }
    }

    Vector3 refineDestinationForUnit(Unit unit, Vector3 destination)
    {
        return new Vector3(destination.x, unit.transform.position.y, destination.z);
    }
}
