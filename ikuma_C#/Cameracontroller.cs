using UnityEngine;

public class CameraController : MonoBehaviour
{
    [Header("追従対象")]
    public Transform targetPlayer;
    private PlayerController_1 playerController;

    [Header("カメラ位置設定")]
    public float distance = 5f;
    public float height = 2f;

    [Header("カメラ追従の滑らかさ")]
    public float followSpeed = 8f;

    [Header("マウス感度")]
    public float mouseSensitivity = 200f;

    [Header("上下視点の制限角度")]
    public float maxLookUpAngle = 60f;
    public float maxLookDownAngle = 30f;

    [Header("視点リセット設定")]
    public float defaultPitch = 10f;

    [Header("壁貫通防止")]
    public LayerMask cameraCollisionLayer;
    public float cameraWallOffset = 0.3f;

    private float currentPitch;
    private float currentYaw = 0f; // プレイヤーの forward からの左右オフセット角度

    void Start()
    {
        currentPitch = defaultPitch;
        if (targetPlayer != null)
            playerController = targetPlayer.GetComponent<PlayerController_1>();
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible   = false;
    }

    void Update()
    {
        if (targetPlayer == null || playerController == null) return;

        currentYaw   += Input.GetAxis("Mouse X") * mouseSensitivity * Time.deltaTime;
        currentPitch -= Input.GetAxis("Mouse Y") * mouseSensitivity * Time.deltaTime;
        currentPitch  = Mathf.Clamp(currentPitch, -maxLookUpAngle, maxLookDownAngle);

        if (Input.GetMouseButtonDown(1))
            currentPitch = defaultPitch;
    }

    void LateUpdate()
    {
        if (targetPlayer == null || playerController == null) return;

        Vector3 up      = playerController.CurrentUp;
        Vector3 forward = playerController.CurrentForward;
        Vector3 right   = playerController.CurrentRight;

        // currentYaw 分だけ up 軸で forward・right を回転させる
        Quaternion yawOffset = Quaternion.AngleAxis(currentYaw, up);
        forward = yawOffset * forward;
        right   = yawOffset * right;

        // Pitch回転をright軸に適用
        Quaternion pitchRotation = Quaternion.AngleAxis(currentPitch, right);
        Vector3 cameraBack = pitchRotation * (-forward);

        // 目標位置を計算
        Vector3 lookAtPoint     = targetPlayer.position + up * (height * 0.5f);
        Vector3 desiredPosition = targetPlayer.position
                                + cameraBack * distance
                                + up * height;

        // 壁貫通防止
        Vector3 directionToCamera = desiredPosition - lookAtPoint;
        float desiredDistance     = directionToCamera.magnitude;

        if (Physics.Raycast(
            lookAtPoint,
            directionToCamera.normalized,
            out RaycastHit hit,
            desiredDistance,
            cameraCollisionLayer))
        {
            float adjustedDistance = Mathf.Max(hit.distance - cameraWallOffset, 0.5f);
            desiredPosition = lookAtPoint + directionToCamera.normalized * adjustedDistance;
        }

        // カメラの向きを計算
        Vector3 cameraForwardDir = (lookAtPoint - desiredPosition).normalized;
        Vector3 cameraUpDir;

        if (Mathf.Abs(Vector3.Dot(cameraForwardDir, up)) > 0.99f)
            cameraUpDir = pitchRotation * forward;
        else
            cameraUpDir = up;

        Quaternion desiredRotation = Quaternion.LookRotation(cameraForwardDir, cameraUpDir);

        float t = 1f - Mathf.Exp(-followSpeed * Time.deltaTime);
        transform.position = Vector3.Lerp(transform.position, desiredPosition, t);
        transform.rotation = Quaternion.Slerp(transform.rotation, desiredRotation, t);
    }
}