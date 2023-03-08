using UnityEngine;

using IsComplete = System.Boolean;

public class Unit : MonoBehaviour
{
    public Animator animator; 

    Order curOrder;

    float rotationSpeed = 5;
    float movementSpeed = 5;

    public void SetOrder(Order order)
    { 
        curOrder = order; 
    }

    public bool IsArrivedAt(Vector3 destination)
    {
        return transform.position == destination;
    }

    public IsComplete RotateTo(Vector3 destination)
    {
        Vector3 direction = destination - transform.position;
        if (Vector3.Angle(transform.forward, direction) < rotationSpeed)
            return true;

        Vector3 newDirection = Vector3.RotateTowards(transform.forward, direction, rotationSpeed * Time.deltaTime, 0);
        transform.rotation = Quaternion.LookRotation(newDirection);
        return false;
    }

    public IsComplete MoveTo(Vector3 destination)
    {
        if (transform.position == destination) 
            return true;

        transform.position = Vector3.MoveTowards(transform.position, destination, movementSpeed * Time.deltaTime);
        animator.SetTrigger("move");
        return false;
    }

    void Start()
    {
        curOrder = new Stop(transform.position);
    }

    void Update()
    {
        executeOrder();
    }

    void executeOrder()
    {
        IsComplete complete = curOrder.ControllUnit(this);
        if (!complete)
            return;

        animator.SetTrigger("stop");
        getNextOrder();
    }

    void getNextOrder()
    {
        Order nextOrder = new Stop(transform.position);
        curOrder = nextOrder;
    }
}
