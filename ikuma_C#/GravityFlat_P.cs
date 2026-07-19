using UnityEngine;

/// <summary>
/// 重力方向の計算のみを担当する
/// isUnlocked が true になるまで動作しない
/// PlayerLevel が Lv3 になったら isUnlocked = true にする
/// </summary>
public class GravityFlat : MonoBehaviour
{
    [Header("タグ設定")]
    public string gravityTag = "GravityWall";

    // PlayerLevel から解放される
    public bool isUnlocked = false;

    // 現在の重力方向（PlayerController が参照する）
    public Vector3 GravityDirection { get; private set; } = Vector3.down;

    void OnCollisionStay(Collision collision)
    {
        // 解放されていない場合は無視
        if (!isUnlocked) return;
        if (!collision.gameObject.CompareTag(gravityTag)) return;

        Vector3 avgNormal = Vector3.zero;
        foreach (ContactPoint contact in collision.contacts)
            avgNormal += contact.normal;

        if (avgNormal.sqrMagnitude > 0.001f)
            GravityDirection = -avgNormal.normalized;
    }

    void OnCollisionExit(Collision collision)
    {
        if (!isUnlocked) return;
        if (!collision.gameObject.CompareTag(gravityTag)) return;
        GravityDirection = Vector3.down;
    }
}