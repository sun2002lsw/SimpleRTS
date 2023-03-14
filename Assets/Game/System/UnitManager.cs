using System.Collections.Generic;
using UnityEngine;

public class UnitManager : MonoBehaviour
{
    List<Unit> allyUnits = new List<Unit>();
    List<Unit> enemyUnits = new List<Unit>();

    void Start()
    {
        foreach (var gameObject in GameObject.FindGameObjectsWithTag("ally"))
            allyUnits.Add(gameObject.GetComponent<Unit>());
        foreach (var gameObject in GameObject.FindGameObjectsWithTag("enemy"))
            enemyUnits.Add(gameObject.GetComponent<Unit>());

        CalculateNearestEnemy();
    }

    void Update()
    {
        CalculateNearestEnemy();
    }

    void CalculateNearestEnemy()
    {
        foreach (Unit allyUnit in allyUnits)
        {
            float shortestDistance = float.MaxValue;
            Unit nearestEnemy = null;

            foreach (Unit enemyUnit in enemyUnits)
            {
                float distance = unitDistance(allyUnit, enemyUnit);
                if (distance < shortestDistance)
                {
                    shortestDistance = distance;
                    nearestEnemy = enemyUnit;
                }
            }

            allyUnit.NearestEnemy = nearestEnemy;
        }

        foreach (Unit enemyUnit in enemyUnits)
        {
            float shortestDistance = float.MaxValue;
            Unit nearestEnemy = null;

            foreach (Unit allyUnit in allyUnits)
            {
                float distance = unitDistance(enemyUnit, allyUnit);
                if (distance < shortestDistance)
                {
                    shortestDistance = distance;
                    nearestEnemy = allyUnit;
                }
            }

            enemyUnit.NearestEnemy = nearestEnemy;
        }
    }

    float unitDistance(Unit u1, Unit u2)
    {
        return Vector3.Distance(u1.Position, u2.Position);
    }
}
