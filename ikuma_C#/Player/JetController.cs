using UnityEngine;

/// <summary>
/// ジェット飛行機能を管理する
/// Player にアタッチ
/// Shift キーで飛行、有効時間がある間のみ使用可能
/// </summary>
public class JetController : MonoBehaviour
{
    [Header("ジェット設定")]
    public float jetForce    = 25f;  // 上昇する力
    public float jetDuration = 10f;  // 飛行可能時間（秒）

    [Header("現在の残り時間（確認用）")]
    public float remainingTime = 0f;

    [Header("取得制限")]
    public float maxJettime = 15f;      // 最大取得数

    // 外部から有効化される
    public bool isUnlocked = false;

    private Rigidbody        rb;
    private PlayerController pc;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        pc = GetComponent<PlayerController>();
    }

    void Update()
    {
        if (!isUnlocked) return;
        if (remainingTime <= 0f) return;

        // Shift キーで飛行
        if (Input.GetKey(KeyCode.LeftShift))
        {
            // 現在の上方向に力を加える
            rb.AddForce(pc.CurrentUp * jetForce, ForceMode.Acceleration);

            // 残り時間を減らす
            remainingTime -= Time.deltaTime;
            remainingTime  = Mathf.Max(remainingTime, 0f);

            Debug.Log($"ジェット飛行中 残り時間: {remainingTime:F1}秒");

            if (remainingTime <= 0f)
                Debug.Log("ジェット燃料切れ！");
        }
    }

    // アイテム取得時に呼ばれる
    public bool AddJetTime(float time)
    {
        if (maxJettime - remainingTime <= 2.0 )
        {
            Debug.Log("これ以上ジェット燃料は取得できません。");
            return false;
        }
        remainingTime += time;
        isUnlocked     = true;
        Debug.Log($"ジェット時間追加！ 残り時間: {remainingTime:F1}秒");
        return true;
    }
}
