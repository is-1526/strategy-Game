using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class PlayerController : MonoBehaviour
{
    [Header("移動設定")]
    public float moveSpeed       = 5f;
    public float dashSpeed       = 10f;
    public float gravityStrength = 20f;
    public float rotationSpeed   = 10f;

    [Header("ジャンプ設定")]
    public float jumpPower = 8f;

    [Header("カメラ参照")]
    public Transform cameraTransform;

    public Vector3 CurrentUp { get; private set; } = Vector3.up;

    public bool canJump = false;
    public bool canDash = false;

    private Rigidbody    rb;
    private GravityFlat  gf;
    private GravitySphere gs;

    // ダッシュ用ダブルタップ
    private float _lastWPressTime  = -1f;
    private float _doubleTapWindow = 0.3f;
    private bool  _isDashing       = false;

    [Header("スタミナ設定")]
    public float maxStamina      = 100f;
    public float staminaDrain    = 20f;
    public float staminaRecovery = 10f;
    public float currentStamina;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        gf = GetComponent<GravityFlat>();
        gs = GetComponent<GravitySphere>();
        rb.useGravity     = false;
        rb.freezeRotation = true;
        currentStamina    = maxStamina;
    }

    bool IsGrounded()
    {
        Vector3 gravityDir = gf != null ? gf.GravityDirection : Vector3.down;
        Vector3 up         = -gravityDir;

        // アクティブな形状の ShapeGroundChecker を取得
        ShapeGroundChecker checker = GetActiveChecker();
        if (checker != null)
        {
            return checker.CheckGrounded(gravityDir, up, transform.right, transform.forward);
        }

        // ShapeGroundChecker がない場合のフォールバック
        Vector3 center = transform.position - up * 0.45f;
        return Physics.Raycast(center, gravityDir, 0.15f);
    }

    // アクティブな形状オブジェクトの ShapeGroundChecker を返す
    ShapeGroundChecker GetActiveChecker()
    {
        ShapeGroundChecker[] checkers = GetComponentsInChildren<ShapeGroundChecker>();
        foreach (ShapeGroundChecker c in checkers)
        {
            if (c.gameObject.activeSelf)
                return c;
        }
        return null;
    }

    void Update()
    {
        // ジャンプ
        if (canJump && IsGrounded() && Input.GetKeyDown(KeyCode.Space))
        {
            rb.AddForce(CurrentUp * jumpPower, ForceMode.Impulse);
        }

        // ダッシュ（ダブルタップ）
        if (canDash)
        {
            if (Input.GetKeyDown(KeyCode.W))
            {
                if (Time.time - _lastWPressTime < _doubleTapWindow && currentStamina > 0f)
                    _isDashing = true;
                _lastWPressTime = Time.time;
            }
            if (Input.GetKeyUp(KeyCode.W))
                _isDashing = false;
            if (currentStamina <= 0f)
                _isDashing = false;

            if (_isDashing)
            {
                currentStamina -= staminaDrain * Time.deltaTime;
                currentStamina  = Mathf.Max(currentStamina, 0f);
            }
            else
            {
                currentStamina += staminaRecovery * Time.deltaTime;
                currentStamina  = Mathf.Min(currentStamina, maxStamina);
            }
        }
    }

    void FixedUpdate()
    {
        // GravitySphere と GravityFlat のどちらが最後に更新されたかで優先度を決める
        // どちらも最後に触れた面の重力方向を保持し続ける
        Vector3 gravityDir;
        if (gs != null && gs.IsAttracting)
            gravityDir = gs.GravityDirection;
        else if (gf != null)
            gravityDir = gf.GravityDirection;
        else
            gravityDir = Vector3.down;

        Vector3 targetUp = -gravityDir;
        CurrentUp        = targetUp;

        // 重力を加算
        rb.AddForce(gravityDir * gravityStrength, ForceMode.Acceleration);

        // WASD入力
        float x = 0f, z = 0f;
        if (Input.GetKey(KeyCode.W)) z =  1f;
        if (Input.GetKey(KeyCode.S)) z = -1f;
        if (Input.GetKey(KeyCode.A)) x = -1f;
        if (Input.GetKey(KeyCode.D)) x =  1f;

        Vector3 camForward = Vector3.ProjectOnPlane(cameraTransform.forward, targetUp).normalized;
        Vector3 camRight   = Vector3.ProjectOnPlane(cameraTransform.right,   targetUp).normalized;
        Vector3 moveDir    = (camForward * z + camRight * x).normalized;

        float speed = (_isDashing && canDash) ? dashSpeed : moveSpeed;

        Vector3 gravityVelocity = Vector3.Project(rb.linearVelocity, gravityDir);
        rb.linearVelocity = gravityVelocity + moveDir * speed;

        Vector3 targetForward = Vector3.ProjectOnPlane(cameraTransform.forward, targetUp).normalized;
        if (targetForward.sqrMagnitude > 0.001f)
        {
            Quaternion targetRot = Quaternion.LookRotation(targetForward, targetUp);
            rb.MoveRotation(Quaternion.Slerp(transform.rotation, targetRot, rotationSpeed * Time.fixedDeltaTime));
        }
    }
}