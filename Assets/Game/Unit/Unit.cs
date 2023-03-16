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

    public Vector3 Position
    { 
        get { return transform.position; }
        private set { transform.position = value; } 
    }

    public Unit NearestEnemy { get; set; }

    public Unit DetectedEnemy
    {
        get 
        {
            if (NearestEnemy != null && Vector3.Distance(NearestEnemy.Position, Position) < UnitData.DetectRange)
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
            Debug.LogAssertionFormat("Invalid unitData object detected: {0}", transform.position);
            Destroy(gameObject);
        }

        hp = UnitData.MaxHP;
        lastAttackTime = DateTime.MinValue;

        navMeshAgent.speed = UnitData.MovementSpeed;
        navMeshAgent.angularSpeed = 180 * UnitData.RotationSpeed;
    }

    void Update()
    {
        executeOrder();
    }

    void executeOrder()
    {
        if (orders.Count == 0)
            orders.Enqueue(new Stop(transform.position));

        Order curOrder = orders.Peek();
        IsComplete complete = curOrder.ControllUnit(this);
        if (complete)
            orders.Dequeue();
    }

    public IsComplete MoveTo(Vector3 destination)
    {
        if (navMeshAgent.destination != destination)
            navMeshAgent.SetDestination(destination);

        if (arrivedAtDestination())
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

    public IsComplete RotateTo(Vector3 destination)
    {
        Vector3 direction = destination - transform.position;
        if (Vector3.Angle(transform.forward, direction) < 10)
            return true;

        float maxRadiansDelta = Mathf.PI * UnitData.RotationSpeed * Time.deltaTime;
        Vector3 newDirection = Vector3.RotateTowards(transform.forward, direction, maxRadiansDelta, 0);
        transform.rotation = Quaternion.LookRotation(newDirection);
        return false;
    }

    public IsComplete AttackUnit(Unit target)
    {
        // Á×¾úÀ¸¸é true

        // approach
        bool canAttackTarget = Vector3.Distance(Position, target.Position) < UnitData.AttackRange;
        if (!canAttackTarget)
        {
            MoveTo(target.Position);
            return false;
        }

        // stop
        if (navMeshAgent.destination != Position)
        {
            navMeshAgent.SetDestination(Position);
            animator.SetBool("isMoving", false);
        }

        // face the target
        if (!RotateTo(target.Position))
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

    IEnumerator giveDamage(Unit target)
    {
        yield return new WaitForSeconds(UnitData.DamageDelay);

        Debug.Log("attack");
    }

    private bool arrivedAtDestination()
    {
        return Vector3.Distance(transform.position, navMeshAgent.destination) < 1;
    }
}
