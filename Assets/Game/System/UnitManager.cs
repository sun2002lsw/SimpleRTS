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
    }

    void Update()
    {
        CalculateNearestEnemy();
    }

    void CalculateNearestEnemy()
    {
        foreach (var allySoldier in allyUnits)
        {
            float shortestDistance = float.MaxValue;
            Unit nearestEnemy = null;

            foreach (var enemySoldier in enemyUnits)
            {
                float distance = unitDistance(allySoldier, enemySoldier);
                if (distance < shortestDistance)
                {
                    shortestDistance = distance;
                    nearestEnemy = enemySoldier;
                }
            }

            setNearestEnemy(allySoldier, nearestEnemy);
        }
    }

    float unitDistance(Unit s1, Unit s2)
    {
        return Vector3.Distance(s1.Position, s2.Position);
    }

    void setNearestEnemy(Unit s1, Unit s2)
    {
        s1.SetNearestEnemy(s2);
        s2.SetNearestEnemy(s1);
    }
}
