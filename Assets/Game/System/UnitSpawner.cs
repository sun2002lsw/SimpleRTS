using System.Collections.Generic;
using UnityEngine;

public class UnitSpawner : MonoBehaviour
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
        instance.GetComponent<Unit>().UnitData = data;
        instance.transform.position = pos;

        lookCenterLine(ref instance);
        setTagByPosition(ref instance);
        setUnitColorByTag(ref instance);

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

    void setUnitColorByTag(ref GameObject obj)
    {
        Unit unit = obj.GetComponent<Unit>();
        if (unit == null)
            return;

        if (obj.tag == "ally")
            setUnitColor(ref obj, Color.blue);
        else if (obj.tag == "enemy")
            setUnitColor(ref obj, Color.red);
        else
            setUnitColor(ref obj, Color.yellow);
    }

    void setUnitColor(ref GameObject obj, Color color)
    {
        Transform armors = obj.transform.Find("Armors");
        if (armors == null)
            return;

        Transform chest = armors.Find("Chest");
        if (chest == null)
            return;

        SkinnedMeshRenderer renderer = chest.GetComponent<SkinnedMeshRenderer>();
        if (renderer == null)
            return;

        renderer.material.color = color;
    }
}
