using UnityEngine;

/// <summary>
/// ジェットアイテム
/// アイテムオブジェクトにアタッチ
/// Collider の IsTrigger を ON にすること
/// Lv2 以上のプレイヤーのみ取得可能
/// </summary>
public class JetItem : MonoBehaviour
{
    [Header("付与する飛行時間（秒）")]
    public float jetTime = 10f;

    void OnTriggerEnter(Collider other)
    {
        // レベル確認
        PlayerLevel playerLevel = other.GetComponentInParent<PlayerLevel>();
        if (playerLevel == null) return;
        if (playerLevel.level < 2)
        {
            Debug.Log("Lv2以上でないとジェットアイテムは取得できません");
            return;
        }

        // JetController に時間を追加
        JetController jet = other.GetComponentInParent<JetController>();
        if (jet == null) return;

        jet.AddJetTime(jetTime);
        Destroy(gameObject);
    }
}
