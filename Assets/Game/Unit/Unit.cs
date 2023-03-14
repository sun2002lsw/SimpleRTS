using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

using IsComplete = System.Boolean;

public class Unit : MonoBehaviour
{
    private UnitData unitData = null;
    public UnitData UnitData 
    { 
        get { return unitData; } 
        set { unitData = value; } 
    }

    private Animator animator;
    private NavMeshAgent navMeshAgent;
    private SpriteRenderer spriteRenderer;
    private Queue<Order> orders = new Queue<Order>();

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
        if (unitData == null)
        {
            Debug.LogAssertionFormat("Invalid unitData object detected: {0}", transform.position);
            Destroy(gameObject);
        }

        navMeshAgent.speed = unitData.MovementSpeed;
        navMeshAgent.angularSpeed = 180 * unitData.RotationSpeed;
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
    public IsComplete RotateTo(Vector3 destination)
    {
        Vector3 direction = destination - transform.position;
        if (Vector3.Angle(transform.forward, direction) < 10)
            return true;

        float maxRadiansDelta = Mathf.PI * unitData.RotationSpeed * Time.deltaTime;
        Vector3 newDirection = Vector3.RotateTowards(transform.forward, direction, maxRadiansDelta, 0);
        transform.rotation = Quaternion.LookRotation(newDirection);
        return false;
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

    private bool arrivedAtDestination()
    {
        return Vector3.Distance(transform.position, navMeshAgent.destination) < 1;
    }
}
