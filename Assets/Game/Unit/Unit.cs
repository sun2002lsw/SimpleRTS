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

    private Animator animator;
    private NavMeshAgent navMeshAgent;
    private SpriteRenderer spriteRenderer;

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

    public void SetSelection(bool selected)
    {
        spriteRenderer.enabled = selected;
    }

    public void GiveOrder(Order order, bool cancelOtherOrders)
    {
        if (cancelOtherOrders)
            orders.Clear();

        orders.Enqueue(order);
    }

    void Awake()
    {
        animator = GetComponent<Animator>();
        navMeshAgent = GetComponent<NavMeshAgent>();
        spriteRenderer = transform.Find("SelectionUI").GetComponent<SpriteRenderer>();
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
            return; // do nothing;

        executeOrder();
    }

    void executeOrder()
    {
        if (orders.Count == 0)
            orders.Enqueue(new Stop(CurPosition));

        Order curOrder = orders.Peek();
        IsComplete complete = curOrder.ControllUnit(this);
        if (complete)
            orders.Dequeue();
    }

    public IsComplete MoveTo(Vector3 destination)
    {
        if (navMeshAgent.destination != destination)
            navMeshAgent.SetDestination(destination);

        // arrived at destination
        if (Vector3.Distance(CurPosition, navMeshAgent.destination) < 1)
        {
            animator.SetBool("isMoving", false);
            return true;
        }

        if (navMeshAgent.velocity.magnitude > 2)
            animator.SetBool("isMoving", true);
        else
            animator.SetBool("isMoving", false);

        return false;
    }

    public IsComplete AttackUnit(Unit target)
    {
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
        {
            navMeshAgent.SetDestination(CurPosition);
            animator.SetBool("isMoving", false);
        }

        // face the target
        if (!rotateTo(target.CurPosition))
            return false;

        // attack
        DateTime nextAttackTime = lastAttackTime.AddSeconds(UnitData.AttackDelay);
        if (nextAttackTime > DateTime.Now)
            return false;

        lastAttackTime = DateTime.Now;
        animator.SetTrigger("attack");
        StartCoroutine(giveDamage(target));
        return false;
    }

    public void DefendPosition()
    {
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

        lastAttackTime = DateTime.Now;
        animator.SetTrigger("attack");
        StartCoroutine(giveDamage(DetectedEnemy));
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

    IEnumerator giveDamage(Unit target)
    {
        yield return new WaitForSeconds(UnitData.DamageDelay);

        target.takeDamage(UnitData.Damage);
    }

    void takeDamage(float damage)
    {
        hp -= damage;
        if (hp <= 0)
        {
            processDeath();
            return;
        }

        animator.SetTrigger("takeDamage");
    }

    void processDeath()
    {
        orders.Clear();
        Destroy(GetComponent<NavMeshAgent>());

        if (tag == "ally")
        {
            UnitController.Instance.DeleteUnit(this);
            UnitManager.Instance.DeleteAllyUnit(this);
        }
        else if (tag == "enemy")
            UnitManager.Instance.DeleteEnemyUnit(this);

        int deathAnimationIdx = UnityEngine.Random.Range(1, 3);
        animator.SetTrigger("death" + deathAnimationIdx);
    }
}
