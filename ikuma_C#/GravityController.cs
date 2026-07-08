using UnityEngine;

/// <summary>
/// 壁・天井に触れると重力方向を変える
/// PlayerLevel のレベルが条件を満たした場合のみ有効
/// Player にアタッチ
/// </summary>
public class GravityController : MonoBehaviour
{
    [Header("重力設定")]
    public float gravityStrength = 20f; // 重力の強さ
    public Vector3 gravityDir = Vector3.down; // 現在の重力方向

    [Header("切り替え設定")]
    public float changeDelay = 0.5f; // 連続切り替え防止の間隔（秒）

    private PlayerLevel   _playerLevel;
    private CharacterController _cc;
    private float _lastChanged = -999f;

    void Start()
    {
        _playerLevel = GetComponent<PlayerLevel>();
        _cc          = GetComponent<CharacterController>();
    }

    void OnControllerColliderHit(ControllerColliderHit hit)
    {
        // レベルが足りない場合は無視
        if (_playerLevel == null || !_playerLevel.canChangeGravity) return;

        // 連続切り替え防止
        if (Time.time - _lastChanged < changeDelay) return;

        Vector3 normal = hit.normal;

        // 軸に平行な面のみ対応（斜め面を無視）
        if (!IsAxisAligned(normal)) return;

        Vector3 newGravity = -normal;

        // すでに同じ方向なら無視
        if (Vector3.Dot(newGravity, gravityDir) > 0.99f) return;

        // 重力方向を変更
        gravityDir     = newGravity;
        _lastChanged   = Time.time;

        // プレイヤーの上方向を新しい重力の逆方向にスムーズ回転
        Vector3 targetUp = -gravityDir;
        transform.rotation = Quaternion.FromToRotation(transform.up, targetUp) * transform.rotation;

        Debug.Log($"重力変更 → {gravityDir}");
    }

    // 軸に平行かチェック（X・Y・Z いずれかにほぼ平行）
    bool IsAxisAligned(Vector3 v)
    {
        return Mathf.Abs(v.x) > 0.9f
            || Mathf.Abs(v.y) > 0.9f
            || Mathf.Abs(v.z) > 0.9f;
    }
}
