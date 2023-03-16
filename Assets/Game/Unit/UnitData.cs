using UnityEngine;

[CreateAssetMenu(fileName = "UnitData", menuName = "Scriptable Object/Unit Data", order = int.MaxValue)]
public class UnitData : ScriptableObject
{
    [SerializeField]
    private string unitName;
    public string UnitName { get { return unitName; } }

    [SerializeField]
    private float maxHP;
    public float MaxHP { get { return maxHP; } }

    [SerializeField]
    private float damage;
    public float Damage { get { return damage; } }

    [SerializeField]
    private float damageDelay;
    public float DamageDelay { get { return damageDelay; } }

    [SerializeField]
    private float attackDelay;
    public float AttackDelay { get { return attackDelay; } }

    [SerializeField]
    private float attackRange;
    public float AttackRange { get { return attackRange; } }

    [SerializeField]
    private float detectRange;
    public float DetectRange { get { return detectRange; } }

    [SerializeField]
    private float rotationSpeed;
    public float RotationSpeed { get { return rotationSpeed; } }

    [SerializeField]
    private float movementSpeed;
    public float MovementSpeed { get { return movementSpeed; } }
}
