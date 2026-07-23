using UnityEngine;

/// <summary>
/// 動く床スクリプト
/// このスクリプトを床オブジェクトにアタッチするだけで動作する
/// 他のスクリプトへの依存なし
/// </summary>
public class MovingPlatform : MonoBehaviour
{
    [Header("移動設定")]
    public Vector3 moveDirection = Vector3.right; // 移動方向
    public float   moveSpeed     = 2f;            // 移動スピード
    public float   moveDistance  = 5f;            // 移動距離

    // 開始位置を記録
    private Vector3 _startPosition;
    private int     _direction = 1; // 1 = 前進、-1 = 後退

    void Start()
    {
        _startPosition = transform.position;
    }

    void Update()
    {
        // 現在の移動量を計算
        Vector3 moved = transform.position - _startPosition;
        float   movedDistance = Vector3.Dot(moved, moveDirection.normalized);

        // 移動距離を超えたら折り返す
        if (movedDistance >= moveDistance)
            _direction = -1;
        else if (movedDistance <= 0f)
            _direction = 1;

        // 移動
        transform.position += moveDirection.normalized * moveSpeed * _direction * Time.deltaTime;
    }

    // プレイヤーが乗ったら一緒に動かす
    void OnCollisionStay(Collision collision)
    {
        // 床の上面に乗っているオブジェクトを動かす
        // Rigidbody があれば MovePosition で、なければ transform で移動
        Rigidbody rb = collision.rigidbody;
        if (rb != null)
        {
            rb.MovePosition(rb.position + moveDirection.normalized * moveSpeed * _direction * Time.deltaTime);
        }
        else
        {
            collision.transform.position += moveDirection.normalized * moveSpeed * _direction * Time.deltaTime;
        }
    }
}
