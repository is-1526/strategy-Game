using UnityEngine;

public class GravityController : MonoBehaviour
{
    [Header("重力設定")]
    public float   gravityStrength = 20f;
    public Vector3 gravityDir      = Vector3.down;

    [Header("切り替え設定")]
    public float changeDelay = 0.5f;

    // 直前に触れた壁の法線（CameraController が参照する）
    public Vector3 LastHitNormal { get; private set; } = Vector3.up;
    public bool    GravityChanged { get; private set; } = false;

    private PlayerLevel _playerLevel;
    private float _lastChanged = -999f;

    void Start()
    {
        _playerLevel = GetComponent<PlayerLevel>();
    }

    void Update()
    {
        // 毎フレームリセット（1フレームだけtrueになる）
        GravityChanged = false;
    }

    void OnControllerColliderHit(ControllerColliderHit hit)
    {
        if (_playerLevel == null || !_playerLevel.canChangeGravity) return;
        if (!hit.gameObject.CompareTag("Wall")) return;
        if (Time.time - _lastChanged < changeDelay) return;

        Vector3 normal = hit.normal;
        if (!IsAxisAligned(normal)) return;

        Vector3 newGravity = -normal;
        if (Vector3.Dot(newGravity, gravityDir) > 0.99f) return;

        gravityDir     = newGravity;
        LastHitNormal  = normal; // 当たった面の法線を保存
        GravityChanged = true;   // 変化したことを通知
        _lastChanged   = Time.time;

        // プレイヤーの上方向を新しい重力の逆方向に回転
        Vector3    targetUp     = -gravityDir;
        Quaternion bodyRotation = Quaternion.FromToRotation(transform.up, targetUp) * transform.rotation;
        transform.rotation = bodyRotation;

        Debug.Log($"重力変更 → {gravityDir} 法線 → {normal}");
    }

    bool IsAxisAligned(Vector3 v)
    {
        return Mathf.Abs(v.x) > 0.9f
            || Mathf.Abs(v.y) > 0.9f
            || Mathf.Abs(v.z) > 0.9f;
    }
}