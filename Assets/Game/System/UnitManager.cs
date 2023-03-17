using System.Collections.Generic;
using UnityEngine;

public class UnitManager : MonoBehaviour
{
    private static UnitManager instance = null;
    public static UnitManager Instance { get { return instance; } }

    HashSet<Unit> allyUnits = new HashSet<Unit>();
    HashSet<Unit> enemyUnits = new HashSet<Unit>();

    public void DeleteAllyUnit(Unit unit) { allyUnits.Remove(unit); }
    public void DeleteEnemyUnit(Unit unit) { enemyUnits.Remove(unit); }

    private void Awake()
    {
        if (instance == null)
            instance = this;
        else
            Destroy(gameObject);
    }

    void Start()
    {
        foreach (var gameObject in GameObject.FindGameObjectsWithTag("ally"))
            allyUnits.Add(gameObject.GetComponent<Unit>());
        foreach (var gameObject in GameObject.FindGameObjectsWithTag("enemy"))
            enemyUnits.Add(gameObject.GetComponent<Unit>());

        calculateNearestEnemy();
    }

    void Update()
    {
        calculateNearestEnemy();
    }

    void calculateNearestEnemy()
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
