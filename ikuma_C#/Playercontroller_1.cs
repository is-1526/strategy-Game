using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class PlayerController_1 : MonoBehaviour
{
    public float moveSpeed = 5f;
    public float dashSpeed = 10f;
    public float jumpPower = 8f;
    public Transform cameraTransform;

    public Vector3 CurrentUp      { get; private set; } = Vector3.up;
    public Vector3 CurrentForward { get; private set; } = Vector3.forward;
    public Vector3 CurrentRight   { get; private set; } = Vector3.right;

    // 追加：直前の実際の移動方向（重力成分を除いた水平移動方向）
    public Vector3 LastMoveDirection { get; private set; } = Vector3.forward;

    private CharacterController cc;
    private GravityController gc;
    private float velocityG = 0f;

    void Start()
    {
        cc = GetComponent<CharacterController>();
        gc = GetComponent<GravityController>();
        Cursor.lockState = CursorLockMode.Locked;
    }

    bool IsGrounded()
    {
        return Physics.Raycast(transform.position, gc.gravityDir, 0.6f);
    }

    void Update()
    {
        CurrentUp      = -gc.gravityDir;
        CurrentForward = Vector3.ProjectOnPlane(transform.forward, CurrentUp).normalized;
        CurrentRight   = Vector3.Cross(CurrentUp, CurrentForward).normalized;

        float x = 0f, z = 0f;
        if (Input.GetKey(KeyCode.W)) z =  1f;
        if (Input.GetKey(KeyCode.S)) z = -1f;
        if (Input.GetKey(KeyCode.A)) x = -1f;
        if (Input.GetKey(KeyCode.D)) x =  1f;

        Vector3 camForward = Vector3.ProjectOnPlane(cameraTransform.forward, CurrentUp).normalized;
        Vector3 camRight   = Vector3.ProjectOnPlane(cameraTransform.right,   CurrentUp).normalized;

        Vector3 moveDir = (camForward * z + camRight * x).normalized;

        // 移動入力がある場合のみ LastMoveDirection を更新
        if (moveDir.sqrMagnitude > 0.001f)
            LastMoveDirection = moveDir;

        float currentSpeed = Input.GetMouseButton(0) ? dashSpeed : moveSpeed;
        cc.Move(moveDir * currentSpeed * Time.deltaTime);

        if (IsGrounded())
        {
            velocityG = -2f;
            if (Input.GetKeyDown(KeyCode.Space))
                velocityG = jumpPower;
        }
        else
        {
            velocityG -= gc.gravityStrength * Time.deltaTime;
        }
        cc.Move(gc.gravityDir * (-velocityG) * Time.deltaTime);
    }
}