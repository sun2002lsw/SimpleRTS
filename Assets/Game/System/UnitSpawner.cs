using System.Collections.Generic;
using UnityEngine;

public class UnitSpawner : MonoBehaviour
{
    [SerializeField]
    private GameObject swordsmanPrefab;
    [SerializeField]
    private UnitData swordsmanData;
    [SerializeField]
    private GameObject magePrefab;
    [SerializeField]
    private UnitData mageData;
    [SerializeField]
    private GameObject assasinPrefab;
    [SerializeField]
    private UnitData assasinData;

    void Awake()
    {
        for (float x = 0; x < 5; x++)
            for (float y = 10; y < 15; y++)
                spawnSymmetryArmy(swordsmanPrefab, swordsmanData, new Vector2(x, y));

        for (float x = 0; x < 5; x++)
            for (float y = 15; y < 17; y++)
                spawnSymmetryArmy(magePrefab, mageData, new Vector2(x, y));

        for (float x = 5; x < 7; x++)
            for (float y = 15; y < 17; y++)
                spawnSymmetryArmy(assasinPrefab, assasinData, new Vector2(x, y));
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
        Unit unit = obj.GetComponent<Unit>();
        if (unit == null) 
            return;

        if (unit.UnitData.UnitName == "swordsman")
            setSwordsmanColor(ref obj, color);
        else if (unit.UnitData.UnitName == "mage")
            setMageColor(ref obj, color);
        else if (unit.UnitData.UnitName == "assasin")
            setAssasinColor(ref obj, color);
    }

    void setSwordsmanColor(ref GameObject obj, Color color)
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

    void setMageColor(ref GameObject obj, Color color)
    {
        Transform skinBody = obj.transform.Find("skn_EarthMage_Body");
        if (skinBody == null)
            return;

        SkinnedMeshRenderer renderer = skinBody.GetComponent<SkinnedMeshRenderer>();
        if (renderer == null)
            return;

        renderer.material.color = color;
    }

    void setAssasinColor(ref GameObject obj, Color color)
    {
        Transform ork = obj.transform.Find("Ork");
        if (ork == null)
            return;

        Transform armor = ork.Find("Armor_7");
        if (armor == null)
            return;

        Transform armorBase = armor.Find("Armor_base");
        if (armorBase == null)
            return;

        SkinnedMeshRenderer renderer = armorBase.GetComponent<SkinnedMeshRenderer>();
        if (renderer == null)
            return;

        renderer.material.color = color;
    }
}
