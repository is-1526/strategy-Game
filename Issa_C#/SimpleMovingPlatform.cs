using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 【概要】
/// 指定した「方向」と「距離」で決まる2地点(開始地点⇔終了地点)を、
/// 一定速度で永遠に往復し続ける、シンプルな移動床スクリプトです。
///
/// 【独立性について】
/// このスクリプトは他のスクリプト(PlayerSurfaceGravity.cs等)を
/// 一切参照・変更しません。動かしたいオブジェクトにそのままアタッチするだけで
/// 単体で完結して動作します。
///
/// 【足場機能について】
/// タグが playerTag と一致するオブジェクトがこの床に接触している間、
/// 床が移動した分だけプレイヤーの位置も直接ずらすことで、
/// 「乗ったら一緒に運ばれる」動きを実現します。
/// これはプレイヤーのRigidbodyの速度(velocity)やAddForceには一切触れないため、
/// 重力方向を法線から計算する既存システムとは独立して動作するはずです。
///
/// 【注意点(既知の未対応ケース)】
/// ・接触判定は「真上に乗っているか」を厳密に区別していません。
///   側面からぶつかった場合も同様に反応します。違和感が出た場合は
///   OnCollisionEnter内の判定条件(接触点の向き等)を絞り込んでください。
/// ・複数のオブジェクトが同時に乗ることを想定し、リストで管理していますが、
///   激しい重なりや特殊なケースまでは検証していません。
/// </summary>
[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(Collider))]
public class SimpleMovingPlatform : MonoBehaviour
{
    [Header("移動設定")]
    [Tooltip("移動する方向(ワールド座標基準)。長さは自動的に正規化されます")]
    [SerializeField] private Vector3 moveDirection = Vector3.right;

    [Tooltip("開始地点からこの方向に何メートル進んだ地点を折り返し地点にするか")]
    [SerializeField] private float moveDistance = 5f;

    [Tooltip("移動速度(メートル/秒)")]
    [SerializeField] private float moveSpeed = 2f;

    [Header("足場設定(プレイヤーを一緒に運ぶ)")]
    [Tooltip("この床がプレイヤーとして認識するタグ名")]
    [SerializeField] private string playerTag = "Player";

    [Tooltip("チェックを入れると、プレイヤーが乗っても一緒に運ばれなくなります(見た目上動くだけの障害物にしたい場合用)")]
    [SerializeField] private bool disableCarryingPlayer = false;

    // 開始地点・終了地点(ゲーム開始時に自動計算)
    private Vector3 startPosition;
    private Vector3 endPosition;

    // 現在、終了地点に向かっているかどうか
    private bool movingToEnd = true;

    // 物理挙動を安定させるためのRigidbody(Kinematicとして使用)
    private Rigidbody rb;

    // 現在この床に乗っているプレイヤーのTransform一覧
    private readonly List<Transform> ridingPlayers = new List<Transform>();

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();

        // 動く床は必ずKinematicにする(そうしないと物理挙動が不安定になり、
        // 衝突判定も正しく動かないため)。既存の設定を上書きするが、
        // これはこのオブジェクト自身の設定であり、他オブジェクトには影響しない。
        rb.isKinematic = true;

        startPosition = rb.position;
        endPosition = startPosition + moveDirection.normalized * moveDistance;
    }

    private void FixedUpdate()
    {
        Vector3 target = movingToEnd ? endPosition : startPosition;
        Vector3 previousPosition = rb.position;

        Vector3 newPosition = Vector3.MoveTowards(
            rb.position,
            target,
            moveSpeed * Time.fixedDeltaTime
        );
        rb.MovePosition(newPosition);

        // 目的地に到達したら、進行方向を反転する
        if (Vector3.Distance(newPosition, target) < 0.01f)
        {
            movingToEnd = !movingToEnd;
        }

        // 足場機能が有効なら、今フレームで動いた分だけ乗っているプレイヤーもずらす
        if (!disableCarryingPlayer && ridingPlayers.Count > 0)
        {
            Vector3 delta = newPosition - previousPosition;
            foreach (Transform rider in ridingPlayers)
            {
                if (rider != null)
                {
                    rider.position += delta;
                }
            }
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.collider.CompareTag(playerTag))
        {
            if (!ridingPlayers.Contains(collision.transform))
            {
                ridingPlayers.Add(collision.transform);
            }
        }
    }

    private void OnCollisionExit(Collision collision)
    {
        if (collision.collider.CompareTag(playerTag))
        {
            ridingPlayers.Remove(collision.transform);
        }
    }

    // シーンビュー上で、往復経路をわかりやすく表示する(レベルデザイン確認用)
    private void OnDrawGizmosSelected()
    {
        Vector3 start = Application.isPlaying ? startPosition : transform.position;
        Vector3 end = Application.isPlaying
            ? endPosition
            : transform.position + moveDirection.normalized * moveDistance;

        Gizmos.color = Color.cyan;
        Gizmos.DrawLine(start, end);
        Gizmos.DrawWireSphere(start, 0.3f);
        Gizmos.DrawWireSphere(end, 0.3f);
    }
}
