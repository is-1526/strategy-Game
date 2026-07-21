using UnityEngine;

/// <summary>
/// 球体・ドーム状オブジェクトに対応した重力スクリプト
/// "Sphere" タグのオブジェクトに触れると
/// その中心に向かって重力がかかる（万有引力のイメージ）
/// 次の "Wall" または "Sphere" タグに触れるまで重力方向を保持する
/// Player にアタッチ
/// </summary>
public class GravitySphere : MonoBehaviour
{
    [Header("タグ設定")]
    public string sphereTag = "Sphere";

    [Header("クールダウン設定")]
    public float changeCooldown = 0.5f;

    [Header("解放フラグ")]
    public bool isUnlocked = false;

    public Vector3 GravityDirection { get; private set; } = Vector3.down;
    public bool    IsAttracting     { get; set; } = false;

    private GravityFlat _gf;

    void Start()
    {
        _gf = GetComponent<GravityFlat>();
    }

    // 現在接触中の球体
    private Transform _currentSphere = null;

    void OnCollisionEnter(Collision collision)
    {
        if (!isUnlocked) return;
        if (!collision.gameObject.CompareTag(sphereTag)) return;
        _currentSphere = collision.transform;
        IsAttracting   = true;
    }

    void OnCollisionExit(Collision collision)
    {
        if (!collision.gameObject.CompareTag(sphereTag)) return;
        // 離れても重力方向は保持（次に触れるまで継続）
        _currentSphere = null;
    }

    void FixedUpdate()
    {
        if (!isUnlocked) return;
        if (!IsAttracting) return;
        if (_currentSphere == null) return;

        // 毎フレーム球の中心からプレイヤーへの方向を計算（滑らか）
        Vector3 toCenter = (_currentSphere.position - transform.position).normalized;
        GravityDirection = toCenter;

        // GravityFlat にも同方向を反映
        if (_gf != null)
            _gf.GravityDirection = toCenter;
    }
}