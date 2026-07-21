using UnityEngine;

/// <summary>
/// 重力方向の計算のみを担当する
/// isUnlocked が true になるまで動作しない
/// </summary>
public class GravityFlat : MonoBehaviour
{
    [Header("タグ設定")]
    public string gravityTag = "Wall";

    [Header("クールダウン設定")]
    public float changeCooldown = 1.0f; // 重力変化後の待機時間（秒）

    public bool isUnlocked = false;

    public Vector3 GravityDirection { get; set; } = Vector3.down;

    private float         _lastChangedTime = -999f;
    private GravitySphere _gs;

    void Start()
    {
        _gs = GetComponent<GravitySphere>();
    }

    void OnCollisionStay(Collision collision)
    {
        if (!isUnlocked) return;
        if (!collision.gameObject.CompareTag(gravityTag)) return;

        // クールダウン中は変化しない
        if (Time.time - _lastChangedTime < changeCooldown) return;

        Vector3 avgNormal = Vector3.zero;
        foreach (ContactPoint contact in collision.contacts)
            avgNormal += contact.normal;

        if (avgNormal.sqrMagnitude < 0.001f) return;

        Vector3 newGravity = -avgNormal.normalized;

        // すでに同じ方向なら無視
        if (Vector3.Dot(newGravity, GravityDirection) > 0.99f) return;

        GravityDirection = newGravity;
        _lastChangedTime = Time.time;

        // 平面に触れたら球状引力を無効にする
        if (_gs != null)
            _gs.IsAttracting = false;

        Debug.Log($"重力変更 → {GravityDirection}");
    }
}