using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class PlayerController_1 : MonoBehaviour
{
    [Header("移動設定")]
    public float moveSpeed  = 5f;
    public float dashSpeed  = 10f;
    public float jumpPower  = 5f;
    
    public float mouseSensitivity = 2f;

    [Header("カメラ")]
    public Transform cameraTransform;

    private CharacterController cc;
    private GravityController gc;
    private float velocityG = 0f;
    private float rotX      = 0f;

    void Start()
    {
        cc = GetComponent<CharacterController>();
        gc = GetComponent<GravityController>();
        Cursor.lockState = CursorLockMode.Locked;
    }

    // プレイヤーの足元にRayを飛ばして地面を検知
    bool IsGrounded()
    {
        // 立方体の中心から足元（0.5f）+ 少し余裕（0.1f）の距離にRayを飛ばす
        return Physics.Raycast(transform.position,  gc.gravityDir, 0.6f);
    }

    void Update()
    {
        // マウスで視点移動
        float mouseX = Input.GetAxis("Mouse X") * 2f;
        float mouseY = Input.GetAxis("Mouse Y") * 2f;
        transform.Rotate(0f, mouseX, 0f);
        rotX -= mouseY;
        rotX  = Mathf.Clamp(rotX, -80f, 80f);
        if (cameraTransform != null)
            cameraTransform.localRotation = Quaternion.Euler(rotX, 0f, 0f);

        // WASD移動（左クリックでダッシュ）
        float x = 0f, z = 0f;
        if (Input.GetKey(KeyCode.W)) z =  1f;
        if (Input.GetKey(KeyCode.S)) z = -1f;
        if (Input.GetKey(KeyCode.A)) x = -1f;
        if (Input.GetKey(KeyCode.D)) x =  1f;

        float currentSpeed = Input.GetMouseButton(0) ? dashSpeed : moveSpeed;


        // 重力方向に垂直な平面上で移動方向を計算
        Vector3 right   = Vector3.Cross(-gc.gravityDir, transform.forward).normalized;
        Vector3 forward = Vector3.Cross(right, -gc.gravityDir).normalized;
        cc.Move((right * x + forward * z).normalized * currentSpeed * Time.deltaTime);


        // ジャンプ・重力
        if (IsGrounded())
        {
            // 地面にいるときは重力をリセット
            velocityG = -2f;
            // スペースでジャンプ
        if (Input.GetKeyDown(KeyCode.Space))
            velocityG = jumpPower;

        }
        else
        {
            velocityG -= gc.gravityStrength * Time.deltaTime;
        }
 
        // 重力方向に移動
        cc.Move(gc.gravityDir * (-velocityG) * Time.deltaTime);

        //アイテムによるジャンプ力増加の場合
        //PlayerController player = other.GetComponent<PlayerController>();
        //player.jumpPower += 0.5f;
    }
}
