using System;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class UnitController : MonoBehaviour
{
    private Camera mainCamera;

    [SerializeField]
    private Texture2D cursor_default;
    [SerializeField]
    private Texture2D cursor_attckMode;
    [SerializeField]
    private Texture2D cursor_ally;
    [SerializeField]
    private Texture2D cursor_enemy;

    private Texture2D currentCursor;
    private bool attackMode = false;
    private bool cancelOtherOrders = false;
    private Vector3 boxingStartPos = Vector3.zero;
    private Vector3 mousePos = Vector3.zero;
    private GameObject mouseObject = null;

    private List<Unit> selectedUnits = new List<Unit>();

    void Awake()
    {
        mainCamera = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<Camera>();
        Cursor.lockState = CursorLockMode.Confined;
    }

    void Start()
    {
        foreach (var gameObject in GameObject.FindGameObjectsWithTag("ally"))
        {
            Unit unit = gameObject.GetComponent<Unit>();
            selectedUnits.Add(unit);
            unit.SetSelection(true);
        }
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.A))
        {
            attackMode = true;
            setMouseCursor(cursor_attckMode);
            return;
        }

        checkMousePos();
        cancelOtherOrders = !Input.GetKey(KeyCode.LeftShift);

        if (Input.GetKeyDown(KeyCode.Mouse0))
            processMouseLeftDown();
        if (Input.GetKeyUp(KeyCode.Mouse0))
            processMouseLeftUp();
        if (Input.GetKeyDown(KeyCode.Mouse1))
            processMouseRightDown();
        else
            processMouseHovering();
    }

    void processMouseLeftDown()
    {
        if (attackMode)
        {
            attackWithFormation();

            attackMode = false;
            setMouseCursor(cursor_default);

            return;
        }

        if (!Input.GetKey(KeyCode.LeftShift))
        {
            foreach (Unit unSelectUnit in selectedUnits)
                unSelectUnit.SetSelection(false);
            selectedUnits.Clear();
        }

        boxingStartPos = mousePos;
    }

    void processMouseLeftUp()
    {
        if (boxingStartPos != Vector3.zero)
        {
            // process boxing select
            boxingStartPos = Vector3.zero;
        }

        if (isCursorOnAlly())
        {
            Unit selectUnit = mouseObject.GetComponent<Unit>();
            selectedUnits.Add(selectUnit);
            selectUnit.SetSelection(true);
        }
    }

    void processMouseRightDown()
    {
        if (attackMode)
        {
            attackMode = false;
            setMouseCursor(cursor_default);

            return;
        }

        if (isCursorOnEnemy())
        {
            attackUnit();
            return;
        }

        moveWithFormation();
    }

    void processMouseHovering()
    {
        if (attackMode)
            return;

        if (isCursorOnAlly())
            setMouseCursor(cursor_ally);
        else if (isCursorOnEnemy())
            setMouseCursor(cursor_enemy);
        else
            setMouseCursor(cursor_default);
    }

    void setMouseCursor(Texture2D cursor)
    {
        if (currentCursor == cursor)
            return;
        else
            currentCursor = cursor;

        Vector2 hotSpot = new Vector2(cursor.width / 10, cursor.height / 10);
        Cursor.SetCursor(cursor, hotSpot, CursorMode.Auto);
    }

    void checkMousePos()
    {
        Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);
        RaycastHit raycastHit;
        if (!Physics.Raycast(ray, out raycastHit))
            return;

        mousePos = raycastHit.point;
        if (raycastHit.collider != null)
            mouseObject = raycastHit.collider.gameObject;
        else
            mouseObject = null;
    }

    bool isCursorOnAlly()
    {
        if (mouseObject == null) 
            return false;

        return mouseObject.tag == "ally";
    }

    bool isCursorOnEnemy()
    {
        if (mouseObject == null)
            return false;

        return mouseObject.tag == "enemy";
    }

    void attackWithFormation()
    {
        Vector3 center = selectedUnitsCenter();

        foreach (var unit in selectedUnits)
        {
            Vector3 destination = mousePos + (unit.transform.position - center);
            unit.GiveOrder(new AttackGround(destination), cancelOtherOrders);
        }
    }

    void moveWithFormation()
    {
        Vector3 center = selectedUnitsCenter();

        foreach (var unit in selectedUnits)
        {
            Vector3 destination = mousePos + unit.transform.position - center;
            unit.GiveOrder(new Move(destination), cancelOtherOrders);
        }
    }

    void attackUnit()
    {
        Unit target = mouseObject.GetComponent<Unit>();
        Order order = new AttackUnit(target);

        foreach (var unit in selectedUnits)
            unit.GiveOrder(order, cancelOtherOrders);
    }

    Vector3 selectedUnitsCenter()
    {
        Vector3 total = Vector3.zero;
        foreach (var unit in selectedUnits)
            total += unit.transform.position;

        return total / selectedUnits.Count;
    }
}
