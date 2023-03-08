using UnityEngine;

public class Unit : MonoBehaviour
{
    Order curOrder;

    void Start()
    {
        curOrder = new Stop(transform);
    }

    void Update()
    {
        executeOrder();
    }

    void executeOrder()
    {
        bool complete = curOrder.ControllUnit(this);
        if (complete)
            getNextOrder();
    }

    void getNextOrder()
    {
        Order nextOrder = new Stop(transform);
        curOrder = nextOrder;
    }
}
