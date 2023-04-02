using UnityEngine;
using Cinemachine;

public class CameraSystem : MonoBehaviour
{
    private static CameraSystem instance = null;
    public static CameraSystem Instance { get { return instance; } }

    [SerializeField]
    private CinemachineVirtualCamera virtualCamera;

    private float FIELD_SIZE = 250;
    private float CAMERA_ROTATE_SPEED = 100;
    private float CAMERA_MOVE_SPEED = 3;
    private float CAMERA_ZOOM_SPEED = 5;
    private float CAMERA_ZOOM_MAX = 100;
    private float CAMERA_ZOOM_MIN = 10;
    private float EDGE_SCROLL_SIZE = 20;

    private Vector3 zoomTargetOffset;
    private bool zoomProcessing;

    private Vector3 savedCursorPos; // save last position before disabling cursor

    public void MoveCamera(Vector3 position)
    {
        // set limitation
        position.x = Mathf.Clamp(position.x, -FIELD_SIZE, FIELD_SIZE);
        position.z = Mathf.Clamp(position.z, -FIELD_SIZE, FIELD_SIZE);

        transform.position = position;
    }

    void Awake()
    {
        if (instance == null)
            instance = this;
        else
            Destroy(gameObject);

        zoomTargetOffset = virtualCamera.GetCinemachineComponent<CinemachineTransposer>().m_FollowOffset;
    }

    void Start()
    {
        ResetCamera();
    }

    private void ResetCamera()
    {
        transform.eulerAngles = new Vector3(0, 0, 0);

        // special setting for Vertical FOV: 60
        float zOffset = zoomTargetOffset.magnitude / 2;
        zoomTargetOffset = new Vector3(0, Mathf.Sqrt(3) * zOffset, -zOffset);
        virtualCamera.GetCinemachineComponent<CinemachineTransposer>().m_FollowOffset = zoomTargetOffset;
    }

    void Update()
    {
        MoveCamera();
        ZoomCamera();
        RotateCamera();

        if (Input.GetKeyDown(KeyCode.Home))
            ResetCamera();
    }

    private void MoveCamera()
    {
        if (Input.GetKey(KeyCode.Mouse0) || Input.GetKey(KeyCode.Mouse1) || Input.GetKey(KeyCode.Mouse2))
            return;

        // get keyboard input
        Vector3 inputDir = new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical"));

        // get mouse edge input
        if (Input.mousePosition.x < EDGE_SCROLL_SIZE) inputDir.x += -1;
        if (Input.mousePosition.y < EDGE_SCROLL_SIZE) inputDir.z += -1;
        if (Input.mousePosition.x > Screen.width - EDGE_SCROLL_SIZE) inputDir.x += +1;
        if (Input.mousePosition.y > Screen.height - EDGE_SCROLL_SIZE) inputDir.z += +1;

        // convert by view angle
        Vector3 moveDir = transform.forward * inputDir.z + transform.right * inputDir.x;

        // set speed by zoom
        moveDir *= CAMERA_MOVE_SPEED * zoomTargetOffset.magnitude * Time.deltaTime;

        // set limitation
        Vector3 newPos = transform.position + moveDir;
        newPos.x = Mathf.Clamp(newPos.x, -FIELD_SIZE, FIELD_SIZE);
        newPos.z = Mathf.Clamp(newPos.z, -FIELD_SIZE, FIELD_SIZE);

        transform.position = newPos;
    }

    private void ZoomCamera()
    {
        setZoom();
        processZoom();
    }

    private void setZoom()
    {
        if (Input.GetKey(KeyCode.Mouse2) || Input.mouseScrollDelta.y == 0)
            return;

        float scroll = Mathf.Clamp(Input.mouseScrollDelta.y, -3, 3);
        zoomTargetOffset = virtualCamera.GetCinemachineComponent<CinemachineTransposer>().m_FollowOffset;

        Vector3 unitVector = zoomTargetOffset.normalized;
        float beforeMagnitude = zoomTargetOffset.magnitude;
        zoomTargetOffset += beforeMagnitude / 2 * unitVector * -scroll;
        if (zoomTargetOffset.y < 0)
        {
            zoomTargetOffset = virtualCamera.GetCinemachineComponent<CinemachineTransposer>().m_FollowOffset;
            return; // inverted camera error. rollback value
        }

        float afterMagnitude = zoomTargetOffset.magnitude;
        if (afterMagnitude > CAMERA_ZOOM_MAX)
            zoomTargetOffset = unitVector * CAMERA_ZOOM_MAX;
        if (afterMagnitude < CAMERA_ZOOM_MIN)
            zoomTargetOffset = unitVector * CAMERA_ZOOM_MIN;

        zoomProcessing = true;
    }

    private void processZoom()
    {
        if (!zoomProcessing)
            return;

        virtualCamera.GetCinemachineComponent<CinemachineTransposer>().m_FollowOffset =
           Vector3.Lerp(virtualCamera.GetCinemachineComponent<CinemachineTransposer>().m_FollowOffset, zoomTargetOffset, CAMERA_ZOOM_SPEED * Time.deltaTime);

        Vector3 currentOffset = virtualCamera.GetCinemachineComponent<CinemachineTransposer>().m_FollowOffset;
        float big = Mathf.Max(currentOffset.magnitude, zoomTargetOffset.magnitude);
        float small = Mathf.Min(currentOffset.magnitude, zoomTargetOffset.magnitude);
        if (small / big > 0.9f) // 90%ก่ complete -> end of process
            zoomProcessing = false;
    }

    private void RotateCamera()
    {
        if (!Input.GetKey(KeyCode.Mouse2) || zoomProcessing)
        {
            cursorVisible();
            return;
        }

        cursorInvisible();

        // left & right
        Vector3 rotateLeftRight = new Vector3(0, Input.GetAxisRaw("Mouse X"), 0);
        transform.eulerAngles += rotateLeftRight * CAMERA_ROTATE_SPEED * Time.deltaTime;

        // up & down
        Vector3 currentUpDown = virtualCamera.GetCinemachineComponent<CinemachineTransposer>().m_FollowOffset;
        float rotateAngle = -Input.GetAxisRaw("Mouse Y") * CAMERA_ROTATE_SPEED * Time.deltaTime;
        Vector3 rotateUpDown = Quaternion.Euler(rotateAngle, 0, 0) * currentUpDown;
        if (rotateUpDown.y < 5 || rotateUpDown.z > 0)
            return; // too low or high angle
        virtualCamera.GetCinemachineComponent<CinemachineTransposer>().m_FollowOffset = rotateUpDown;
    }

    private void cursorInvisible()
    {
        if (!Cursor.visible)
            return;

        savedCursorPos = Input.mousePosition;
        Cursor.visible = false;
    }

    private void cursorVisible()
    {
        if (Cursor.visible)
            return;

        Vector2 pos = new Vector2(savedCursorPos.x, savedCursorPos.y);
        UnityEngine.InputSystem.Mouse.current.WarpCursorPosition(pos);
        Cursor.visible = true;
    }
}
