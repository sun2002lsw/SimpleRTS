using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArmyDeploy : MonoBehaviour
{
    [SerializeField]
    private GameObject swordsmanPrefab;
    [SerializeField]
    private UnitData swordsmanData;

    void Awake()
    {
        for (float x = 0; x < 5; x++)
            for (float y = 10; y < 15; y++)
                spawnSymmetryArmy(swordsmanPrefab, swordsmanData, new Vector2(x, y));
    }

    void spawnSymmetryArmy(GameObject obj, UnitData data, Vector2 pos)
    {
        if (pos.x < 0 || pos.y < 10)
            return;

        // all symmetry positions
        List<Vector3> positions = new List<Vector3>
        {
            new Vector3(pos.x, 1, pos.y),
            new Vector3(pos.x, 1, -pos.y)
        };

        if (pos.x != 0)
        {
            positions.Add(new Vector3(-pos.x, 1, pos.y));
            positions.Add(new Vector3(-pos.x, 1, -pos.y));
        }

        // spawn units
        foreach (var position in positions)
            spawnUnit(obj, data, position);
    }

    GameObject spawnUnit(GameObject obj, UnitData data, Vector3 pos)
    {
        GameObject instance = Instantiate(obj);

        instance.transform.position = pos;
        lookCenterLine(ref instance);
        setTagByPosition(ref instance);
        //setColorByTag(ref instance);

        Unit unit = instance.GetComponent<Unit>();
        unit.UnitData = data;

        return instance;
    }

    void lookCenterLine(ref GameObject obj)
    {
        Vector3 pos = obj.transform.position;
        if (pos.z > 0)
            obj.transform.rotation = Quaternion.LookRotation(new Vector3(0, pos.y, -pos.z));
    }

    void setTagByPosition(ref GameObject obj)
    {
        if (obj.transform.position.z > 0)
            obj.tag = "enemy";
        else
            obj.tag = "ally";
    }

    void setColorByTag(ref GameObject obj)
    {
        if (obj.tag == "enemy")
        {
            Renderer renderer = obj.GetComponent<Renderer>();
            renderer.material.color = Color.red;
        }
    }
}
