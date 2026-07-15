using UnityEngine;

public class CameraController : MonoBehaviour
{
    [Header("参照")]
    public Transform           player;
    public GravityController   gc;
    public PlayerController_1  pc;

    [Header("カメラ位置設定")]
    public float distance = 5f;
    public float height   = 2f;

    [Header("カメラ追従の滑らかさ")]
    public float followSpeed = 8f;

    [Header("マウス感度")]
    public float mouseSensitivity = 200f;

    [Header("上下視点の制限角度")]
    public float maxLookUpAngle   = 60f;
    public float maxLookDownAngle = 30f;

    [Header("視点リセット設定")]
    public float defaultPitch = 10f;

    [Header("壁貫通防止")]
    public LayerMask cameraCollisionLayer;
    public float     cameraWallOffset = 0.3f;

    private float      _currentPitch;
    private Quaternion _yawRotation = Quaternion.identity;
    private Vector3    _lastUp      = Vector3.up;

    void Start()
    {
        _currentPitch    = defaultPitch;
        _lastUp          = pc != null ? pc.CurrentUp : Vector3.up;
        _yawRotation     = Quaternion.LookRotation(
            Vector3.ProjectOnPlane(player.forward, _lastUp).normalized, _lastUp);
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible   = false;
    }

    void Update()
    {
        if (player == null || pc == null) return;

        Vector3 up = pc.CurrentUp;

        // 重力方向が変わったら Yaw をリセット
        if (Vector3.Dot(_lastUp, up) < 0.99f)
        {
            // 直前の実際の移動方向を新しい up 平面に投影してカメラ正面にする
            Vector3 lastMove    = pc.LastMoveDirection;
            Vector3 moveOnPlane = Vector3.ProjectOnPlane(lastMove, up);

            if (moveOnPlane.sqrMagnitude > 0.001f)
            {
                _yawRotation = Quaternion.LookRotation(moveOnPlane.normalized, up);
            }

            _lastUp = up;
        }

        // マウス左右で Yaw を up 軸基準で回転
        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity * Time.deltaTime;
        _yawRotation = Quaternion.AngleAxis(mouseX, up) * _yawRotation;

        // マウス上下で Pitch を変化
        _currentPitch -= Input.GetAxis("Mouse Y") * mouseSensitivity * Time.deltaTime;
        _currentPitch  = Mathf.Clamp(_currentPitch, -maxLookUpAngle, maxLookDownAngle);

        if (Input.GetMouseButtonDown(1))
            _currentPitch = defaultPitch;
    }

    void LateUpdate()
    {
        if (player == null || pc == null) return;

        Vector3 up = pc.CurrentUp;

        Vector3 forward = Vector3.ProjectOnPlane(_yawRotation * Vector3.forward, up).normalized;
        Vector3 right   = Vector3.Cross(up, forward).normalized;

        Quaternion pitchRotation = Quaternion.AngleAxis(_currentPitch, right);
        Vector3    cameraBack    = pitchRotation * (-forward);

        Vector3 lookAtPoint     = player.position + up * (height * 0.5f);
        Vector3 desiredPosition = player.position + cameraBack * distance + up * height;

        Vector3    directionToCamera = desiredPosition - lookAtPoint;
        float      desiredDistance   = directionToCamera.magnitude;
        RaycastHit hit;
        if (Physics.Raycast(
                lookAtPoint,
                directionToCamera.normalized,
                out hit,
                desiredDistance,
                cameraCollisionLayer))
        {
            float adjustedDistance = Mathf.Max(hit.distance - cameraWallOffset, 0.5f);
            desiredPosition = lookAtPoint + directionToCamera.normalized * adjustedDistance;
        }

        Vector3 cameraForwardDir = (lookAtPoint - desiredPosition).normalized;
        Vector3 cameraUpDir      = Mathf.Abs(Vector3.Dot(cameraForwardDir, up)) > 0.99f
                                 ? pitchRotation * forward
                                 : up;

        Quaternion desiredRotation = Quaternion.LookRotation(cameraForwardDir, cameraUpDir);

        float t = 1f - Mathf.Exp(-followSpeed * Time.deltaTime);
        transform.position = Vector3.Lerp(transform.position, desiredPosition, t);
        transform.rotation = Quaternion.Slerp(transform.rotation, desiredRotation, t);
    }
}