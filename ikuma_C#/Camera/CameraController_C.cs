using UnityEngine;

public class CameraController : MonoBehaviour
{
    [Header("追従対象")]
    public Transform targetPlayer;
    private PlayerController playerController;

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

    private float      currentPitch;
    // YawをQuaternionで管理することで軸変化に対応
    private Quaternion _yawRotation = Quaternion.identity;
    private Vector3    _lastUp      = Vector3.up;

    void Start()
    {
        currentPitch = defaultPitch;

        if (targetPlayer != null)
        {
            playerController = targetPlayer.GetComponent<PlayerController>();
            _lastUp          = playerController.CurrentUp;
            _yawRotation     = Quaternion.LookRotation(
                Vector3.ProjectOnPlane(targetPlayer.forward, _lastUp).normalized, _lastUp);
        }

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible   = false;
    }

    void Update()
    {
        if (targetPlayer == null || playerController == null) return;

        Vector3 currentUp = playerController.CurrentUp;

        // 重力変化を検知したら視点をリセット
        if (Vector3.Dot(_lastUp, currentUp) < 0.99f)
        {
            // Pitchをリセットして真上を向かせる
            currentPitch = defaultPitch;

            // Yawをプレイヤーの前方に合わせてリセット
            Vector3 playerFwd = Vector3.ProjectOnPlane(targetPlayer.forward, currentUp);
            if (playerFwd.sqrMagnitude > 0.001f)
                _yawRotation = Quaternion.LookRotation(playerFwd.normalized, currentUp);

            _lastUp = currentUp;
        }

        // マウス左右：up軸基準でYawを回転
        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity * Time.deltaTime;
        _yawRotation = Quaternion.AngleAxis(mouseX, currentUp) * _yawRotation;

        // マウス上下：Pitchを変化
        currentPitch -= Input.GetAxis("Mouse Y") * mouseSensitivity * Time.deltaTime;
        currentPitch  = Mathf.Clamp(currentPitch, -maxLookUpAngle, maxLookDownAngle);

        if (Input.GetMouseButtonDown(1))
            currentPitch = defaultPitch;
    }

    void LateUpdate()
    {
        if (targetPlayer == null || playerController == null) return;

        Vector3 up = playerController.CurrentUp;

        // _yawRotation から forward を取得
        Vector3 forward = Vector3.ProjectOnPlane(_yawRotation * Vector3.forward, up).normalized;
        if (forward.sqrMagnitude < 0.001f)
        {
            Vector3 fallback = Mathf.Abs(Vector3.Dot(up, Vector3.forward)) < 0.99f
                ? Vector3.forward : Vector3.right;
            forward = Vector3.ProjectOnPlane(fallback, up).normalized;
        }
        Vector3 right = Vector3.Cross(up, forward).normalized;

        Quaternion pitchRotation = Quaternion.AngleAxis(currentPitch, right);
        Vector3    cameraBack    = pitchRotation * (-forward);

        Vector3 lookAtPoint     = targetPlayer.position + up * (height * 0.5f);
        Vector3 desiredPosition = targetPlayer.position + cameraBack * distance + up * height;

        Vector3    directionToCamera = desiredPosition - lookAtPoint;
        float      desiredDistance   = directionToCamera.magnitude;
        RaycastHit hit;
        if (Physics.Raycast(lookAtPoint, directionToCamera.normalized, out hit,
                            desiredDistance, cameraCollisionLayer))
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