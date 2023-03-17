using System;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using static UnityEngine.UI.CanvasScaler;

public class UnitController : MonoBehaviour
{
    private static UnitController instance = null;
    public static UnitController Instance { get { return instance; } }

    private Camera mainCamera;
    private AudioSource audioSource;

    private Texture2D currentCursor;
    [SerializeField]
    private Texture2D cursor_default;
    [SerializeField]
    private Texture2D cursor_attckMode;
    [SerializeField]
    private Texture2D cursor_ally;
    [SerializeField]
    private Texture2D cursor_enemy;

    private Vector2 dragSelectFrom = Vector2.zero;
    private Vector2 curMousePos = Vector2.zero;
    [SerializeField]
    private RectTransform selectionBox;
    private HashSet<Unit> selectionBoxUnits = new HashSet<Unit>();

    private bool attackMode = false;
    private bool cancelOtherOrders = false;
    private Vector3 mouseObjectPos = Vector3.zero;
    private GameObject mouseObject = null;

    private HashSet<Unit> selectableUnits = new HashSet<Unit>();
    private HashSet<Unit> selectedUnits = new HashSet<Unit>();

    public void DeleteUnit(Unit unit)
    {
        selectableUnits.Remove(unit);
        selectedUnits.Remove(unit);
    }

    void Awake()
    {
        if (instance == null)
            instance = this;
        else
            Destroy(gameObject);

        mainCamera = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<Camera>();
        audioSource = GetComponent<AudioSource>();
        selectionBox = transform.Find("Canvas").Find("SelectionBox").GetComponent<RectTransform>();
        Cursor.lockState = CursorLockMode.Confined;
    }

    void Start()
    {
        var allyObjects = GameObject.FindGameObjectsWithTag("ally");
        foreach (var allyObject in allyObjects)
            if (allyObject.GetComponent<Unit>() != null)
                selectableUnits.Add(allyObject.GetComponent<Unit>());
    }

    void Update()
    {
        cancelOtherOrders = !Input.GetKey(KeyCode.LeftShift);

        processKeyboardInput();
        processMouseInput();
    }

    void processKeyboardInput()
    {
        if (Input.GetKeyDown(KeyCode.A))
        {
            attackMode = true;
            setMouseCursor(cursor_attckMode);
            return;
        }

        if (Input.GetKeyDown(KeyCode.S))
        {
            orderSelectedUnitsWithSound(unit => new Stop(unit.CurPosition));
            return;
        }

        if (Input.GetKeyDown(KeyCode.H))
        {
            orderSelectedUnitsWithSound(unit => new Hold());
            return;
        }
    }

    void processMouseInput()
    {
        checkMousePos();

        if (Input.GetKeyDown(KeyCode.Mouse0))
            processMouseLeftDown();
        else if (Input.GetKeyUp(KeyCode.Mouse0))
            processMouseLeftUp();
        else if (Input.GetKeyDown(KeyCode.Mouse1))
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

        dragSelectFrom = curMousePos;
        setMouseCursor(cursor_default);
    }

    void processMouseLeftUp()
    {
        if (dragSelectFrom == curMousePos && isCursorOnAlly())
        {
            Unit selectUnit = mouseObject.GetComponent<Unit>();
            if (!selectedUnits.Contains(selectUnit))
            {
                selectedUnits.Add(selectUnit);
                selectUnit.SetSelection(true);
                selectUnit.PlaySelectionVoice(audioSource);
            }
            else if (Input.GetKey(KeyCode.LeftShift))
            {
                selectedUnits.Remove(selectUnit);
                selectUnit.SetSelection(false);
            }
        }
        else if (dragSelectFrom != Vector2.zero)
            completeBoxingSelect();

        dragSelectFrom = Vector2.zero;
    }

    void processMouseRightDown()
    {
        if (dragSelectFrom != Vector2.zero)
        {
            cancelBoxingSelect();
            return;
        }

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
        if (dragSelectFrom != Vector2.zero)
        {
            processBoxingSelect();
            return;
        }

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
        curMousePos = Input.mousePosition;

        Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);
        RaycastHit raycastHit;
        if (!Physics.Raycast(ray, out raycastHit))
            return;

        mouseObjectPos = raycastHit.point;
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
        orderSelectedUnitsWithSound(unit => new AttackGround(mouseObjectPos + unit.transform.position - center));
    }

    void moveWithFormation()
    {
        Vector3 center = selectedUnitsCenter();
        orderSelectedUnitsWithSound(unit => new Move(mouseObjectPos + unit.transform.position - center));
    }

    void attackUnit()
    {
        Unit target = mouseObject.GetComponent<Unit>();
        orderSelectedUnitsWithSound(unit => new AttackUnit(target));
    }

    Vector3 selectedUnitsCenter()
    {
        Vector3 total = Vector3.zero;
        foreach (var unit in selectedUnits)
            total += unit.transform.position;

        return total / selectedUnits.Count;
    }

    void orderSelectedUnitsWithSound(Func<Unit, Order> orderCreator)
    {
        bool alreadySound = false;

        foreach (var unit in selectedUnits)
        {
            Order order = orderCreator(unit);

            if (!alreadySound)
            {
                unit.PlayOrderSound(order, audioSource);
                alreadySound = true;
            }

            unit.GiveOrder(order, cancelOtherOrders);
        }
    }

    void processBoxingSelect()
    {
        if (dragSelectFrom == curMousePos)
            return;

        // draw selection box
        float width = curMousePos.x - dragSelectFrom.x;
        float height = curMousePos.y - dragSelectFrom.y;

        Vector2 newAnchoredPosition = dragSelectFrom + new Vector2(width / 2, height / 2);
        if (newAnchoredPosition == selectionBox.anchoredPosition)
            return;

        selectionBox.anchoredPosition = newAnchoredPosition;
        selectionBox.sizeDelta = new Vector2(Mathf.Abs(width), Mathf.Abs(height));

        // check boxing units
        Bounds bounds = new Bounds(selectionBox.anchoredPosition, selectionBox.sizeDelta);
        HashSet<Unit> newBoxingUnits = new HashSet<Unit>();

        foreach (var unit in selectableUnits)
        {
            Vector2 unitScreenPos = mainCamera.WorldToScreenPoint(unit.transform.position);
            if (bounds.Contains(unitScreenPos))
                newBoxingUnits.Add(unit);
        }

        // extracted units
        HashSet<Unit> extractedUnits = new HashSet<Unit>();
        foreach (Unit unit in selectionBoxUnits)
            if (!newBoxingUnits.Contains(unit))
                extractedUnits.Add(unit);

        foreach (Unit unit in extractedUnits)
            if (!selectedUnits.Contains(unit))
            {
                unit.SetSelection(false);
                selectionBoxUnits.Remove(unit);
            }

        // new added units
        foreach (Unit unit in newBoxingUnits)
            if (!selectionBoxUnits.Contains(unit))
            {
                unit.SetSelection(true);
                selectionBoxUnits.Add(unit);
            }
    }

    void completeBoxingSelect()
    {
        foreach (Unit unit in selectionBoxUnits)
            selectedUnits.Add(unit);

        selectionBoxUnits.Clear();
        selectionBox.sizeDelta = Vector2.zero;

        if (selectedUnits.Count > 0)
            selectedUnits.First<Unit>().PlaySelectionVoice(audioSource);
    }

    void cancelBoxingSelect()
    {
        foreach (Unit unit in selectionBoxUnits)
            if (!selectedUnits.Contains(unit))
                unit.SetSelection(false);

        selectionBoxUnits.Clear();
        selectionBox.sizeDelta = Vector2.zero;
    }
}
