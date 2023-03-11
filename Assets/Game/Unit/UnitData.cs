using UnityEngine;

[CreateAssetMenu(fileName = "UnitData", menuName = "Scriptable Object/Unit Data", order = int.MaxValue)]
public class UnitData : ScriptableObject
{
    [SerializeField]
    private string unitName;
    public string UnitName { get { return unitName; } }

    [SerializeField]
    private string maxHP;
    public string MaxHP { get { return maxHP; } }

    [SerializeField]
    private float damage;
    public float Damage { get { return damage; } }

    [SerializeField]
    private float attackRange;
    public float AttackRange { get { return attackRange; } }
}
