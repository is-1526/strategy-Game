using UnityEngine;

/// <summary>
/// 各形状オブジェクトにアタッチして接地判定の設定を持たせる
/// CubeShape・RectangleShape それぞれにアタッチ
/// </summary>
public class ShapeGroundChecker : MonoBehaviour
{
    [Header("接地判定設定")]
    public float rayLength  = 0.15f; // Rayの長さ
    public float rayOffset  = 0.4f;  // 四隅のオフセット
    public float bottomOffset = 0.45f; // 底面からのオフセット

    /// <summary>
    /// 現在の重力方向に対して接地しているか判定する
    /// PlayerController から呼ばれる
    /// </summary>
    public bool CheckGrounded(Vector3 gravityDir, Vector3 up, Vector3 right, Vector3 forward)
    {
        // 底面の少し上を判定開始位置にする
        Vector3 center = transform.position - up * bottomOffset;

        // 中央
        if (Physics.Raycast(center, gravityDir, rayLength)) return true;

        // 四隅
        if (Physics.Raycast(center + right   * rayOffset + forward * rayOffset, gravityDir, rayLength)) return true;
        if (Physics.Raycast(center + right   * rayOffset - forward * rayOffset, gravityDir, rayLength)) return true;
        if (Physics.Raycast(center - right   * rayOffset + forward * rayOffset, gravityDir, rayLength)) return true;
        if (Physics.Raycast(center - right   * rayOffset - forward * rayOffset, gravityDir, rayLength)) return true;

        return false;
    }
}
