using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

using IsComplete = System.Boolean;

public class Unit : MonoBehaviour
{
    public UnitData UnitData { get; set; }

    private DateTime lastAttackTime;
    private float hp;
    private Queue<Order> orders = new Queue<Order>();

    private SpriteRenderer spriteRenderer;
    private NavMeshAgent navMeshAgent;
    private UnitAnimation unitAnimation;
    private UnitSound unitSound;

    public Vector3 CurPosition
    { 
        get { return transform.position; }
        private set { transform.position = value; } 
    }

    public Unit NearestEnemy { get; set; }

    public Unit DetectedEnemy
    {
        get 
        {
            if (NearestEnemy != null && Vector3.Distance(NearestEnemy.CurPosition, CurPosition) < UnitData.DetectRange)
                return NearestEnemy;
            else
                return null;
        }
        private set { }
    }

    public void Roar() { unitAnimation.Roar(); }
    public void SetSelection(bool selected) { spriteRenderer.enabled = selected; }
    public void PlaySelectionVoice(AudioSource system) { unitSound.PlaySelectVoiceSound(system); }
    public void PlayOrderSound(Order order, AudioSource system) { order.PlayOrderSound(unitSound, system); }

    public void GiveOrder(Order order, bool cancelOtherOrders)
    {
        if (cancelOtherOrders)
            orders.Clear();

        orders.Enqueue(order);
    }

    void Awake()
    {
        spriteRenderer = transform.Find("SelectionUI").GetComponent<SpriteRenderer>();
        navMeshAgent = GetComponent<NavMeshAgent>();
        unitAnimation = GetComponent<UnitAnimation>();
        unitSound = GetComponent<UnitSound>();
    }

    void Start()
    {
        if (UnitData == null)
        {
            Debug.LogAssertionFormat("Invalid unitData object detected: {0}", CurPosition);
            Destroy(gameObject);
        }

        hp = UnitData.MaxHP;
        lastAttackTime = DateTime.MinValue;

        navMeshAgent.speed = UnitData.MovementSpeed;
        navMeshAgent.angularSpeed = 180 * UnitData.RotationSpeed;
    }

    void Update()
    {
        if (hp <= 0)
            return; // do nothing

        executeOrder();
    }

    void executeOrder()
    {
        if (orders.Count == 0)
        {
            orders.Enqueue(new Stop(CurPosition));
            navMeshAgent.avoidancePriority = Constants.AVOIDANCE_PRIORITY_NORMAL;
        }

        Order curOrder = orders.Peek();
        IsComplete complete = curOrder.ControllUnit(this);
        if (complete)
            orders.Dequeue();
    }

    public IsComplete MoveTo(Vector3 destination)
    {
        navMeshAgent.avoidancePriority = Constants.AVOIDANCE_PRIORITY_MOVE;

        if (unitAnimation.IsHolding())
            unitAnimation.Hold(false);
        if (unitAnimation.IsAttacking())
            return false;

        if (navMeshAgent.destination != destination)
            navMeshAgent.SetDestination(destination);

        // arrived at destination
        return Vector3.Distance(CurPosition, navMeshAgent.destination) < 1;
    }

    public IsComplete AttackUnit(Unit target)
    {
        navMeshAgent.avoidancePriority = Constants.AVOIDANCE_PRIORITY_MOVE;

        // already dead
        if (target.hp <= 0)
            return true;

        // approach
        bool canAttackTarget = Vector3.Distance(CurPosition, target.CurPosition) < UnitData.AttackRange;
        if (!canAttackTarget)
        {
            MoveTo(target.CurPosition);
            return false;
        }

        // stop
        if (navMeshAgent.destination != CurPosition)
            navMeshAgent.SetDestination(CurPosition);

        // face the target
        if (!rotateTo(target.CurPosition))
            return false;

        // attack
        DateTime nextAttackTime = lastAttackTime.AddSeconds(UnitData.AttackDelay);
        if (nextAttackTime > DateTime.Now)
            return false;

        MeleeAttack(target);
        unitAnimation.Attack();
        lastAttackTime = DateTime.Now;
        return false;
    }

    public void DefendPosition()
    {
        navMeshAgent.avoidancePriority = Constants.AVOIDANCE_PRIORITY_STOP;

        // stop
        if (navMeshAgent.destination != CurPosition)
            navMeshAgent.SetDestination(CurPosition);
        if (!unitAnimation.IsHolding())
            unitAnimation.Hold(true);

        if (DetectedEnemy == null)
            return;

        // face the target
        if (!rotateTo(DetectedEnemy.CurPosition))
            return;

        // wait for range
        bool canAttackTarget = Vector3.Distance(CurPosition, DetectedEnemy.CurPosition) < UnitData.AttackRange;
        if (!canAttackTarget)
            return;

        // attack
        DateTime nextAttackTime = lastAttackTime.AddSeconds(UnitData.AttackDelay);
        if (nextAttackTime > DateTime.Now)
            return;

        MeleeAttack(DetectedEnemy);
        unitAnimation.Attack();
        lastAttackTime = DateTime.Now;
    }

    IsComplete rotateTo(Vector3 destination)
    {
        Vector3 direction = destination - CurPosition;
        if (Vector3.Angle(transform.forward, direction) < 10)
            return true;

        float maxRadiansDelta = Mathf.PI * UnitData.RotationSpeed * Time.deltaTime;
        Vector3 newDirection = Vector3.RotateTowards(transform.forward, direction, maxRadiansDelta, 0);
        transform.rotation = Quaternion.LookRotation(newDirection);
        return false;
    }

    void MeleeAttack(Unit target)
    {
        navMeshAgent.avoidancePriority = Constants.AVOIDANCE_PRIORITY_STOP;
        StartCoroutine(giveMeleeDamage(target));
    }

    IEnumerator giveMeleeDamage(Unit target)
    {
        yield return new WaitForSeconds(UnitData.DamageDelay);
        if (hp <= 0)
            yield break; // dead unit

        navMeshAgent.avoidancePriority = Constants.AVOIDANCE_PRIORITY_NORMAL;
        target.takeDamage(UnitData.Damage);
    }

    void takeDamage(float damage)
    {
        hp -= damage;
        if (hp > 0)
            unitAnimation.TakeDamage();
        else
            processDeath();
    }

    void processDeath()
    {
        orders.Clear();
        SetSelection(false);
        unitAnimation.Death();

        if (tag == "ally")
        {
            UnitController.Instance.DeleteUnit(this);
            UnitManager.Instance.DeleteAllyUnit(this);
        }
        else if (tag == "enemy")
            UnitManager.Instance.DeleteEnemyUnit(this);

        Destroy(GetComponent<CapsuleCollider>());
        Destroy(GetComponent<NavMeshAgent>());
        Destroy(GetComponent<UnitAnimation>());
    }
}

static class Constants
{
    public const int AVOIDANCE_PRIORITY_NORMAL = 50;
    public const int AVOIDANCE_PRIORITY_MOVE = 30;
    public const int AVOIDANCE_PRIORITY_STOP = 0;
}
