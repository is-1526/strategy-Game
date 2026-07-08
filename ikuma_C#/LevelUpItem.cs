using UnityEngine;

/// <summary>
/// 触れると経験値を加算して消えるアイテム
/// アイテムオブジェクトにアタッチ
/// ColliderのIsTriggerをONにすること
/// </summary>
public class LevelUpItem : MonoBehaviour
{
    public int expAmount = 100; // 取得時に加算する経験値

    void OnTriggerEnter(Collider other)
    {
        // プレイヤーに触れたか確認
        PlayerLevel playerLevel = other.GetComponent<PlayerLevel>();
        if (playerLevel == null) return;

        // 経験値を加算
        playerLevel.AddExp(expAmount);

        // アイテムを消す
        Destroy(gameObject);
    }
}
