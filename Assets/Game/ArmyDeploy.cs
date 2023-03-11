using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArmyDeploy : MonoBehaviour
{
    [SerializeField]
    private GameObject swordsmanPrefab;
    [SerializeField]
    private UnitData swordsmanData;

    void Start()
    {
        Vector3 v1 = new Vector3(-1, 1, -1);
        Unit s1 = spawnSwordsman(v1);
        s1.tag = "ally";

        Vector3 v2 = new Vector3(1, 1, 1);
        Unit s2 = spawnSwordsman(v2);
        s2.tag = "enemy";
    }

    Unit spawnSwordsman(Vector3 pos)
    {
        return spawnUnit(swordsmanPrefab, swordsmanData, pos);
    }

    Unit spawnUnit(GameObject obj, UnitData data, Vector3 pos)
    {
        Unit unit = Instantiate(obj).GetComponent<Unit>();
        unit.UnitData = data;
        
        unit.transform.position = pos;
        return unit;
    }
}
