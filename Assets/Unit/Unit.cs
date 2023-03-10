using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

using IsComplete = System.Boolean;

public class Unit : MonoBehaviour
{
    private Animator animator;
    private NavMeshAgent navMeshAgent;
    private List<Order> orders = new List<Order>();

    public void SetOrder(Order order)
    {
        orders.Clear();
        orders.Add(order);
    }

    public void AddOrder(Order order)
    {
        orders.Add(order);
    }

    private void Awake()
    {
        animator = GetComponent<Animator>();
        navMeshAgent = GetComponent<NavMeshAgent>();
    }

    void Start()
    {
        // do nothing
    }

    void Update()
    {
        Debug.LogFormat("current speed: {0}", navMeshAgent.velocity.magnitude);

        executeOrder();
    }

    void executeOrder()
    {
        if (orders.Count == 0)
            orders.Add(new Stop(transform.position));

        Order curOrder = orders[0];

        IsComplete complete = curOrder.ControllUnit(this);
        if (complete)
            orders.Remove(curOrder);
    }

    public bool MoveToDestination(Vector3 destination)
    {
        if (navMeshAgent.destination != destination)
            navMeshAgent.SetDestination(destination);

        if (arrivedAtDestination())
        {
            animator.SetBool("isMoving", false);
            return true;
        }

        animator.SetBool("isMoving", true);
        return false;
    }

    private bool arrivedAtDestination()
    {
        return Vector3.Distance(transform.position, navMeshAgent.destination) < 0.5;
    }
}
