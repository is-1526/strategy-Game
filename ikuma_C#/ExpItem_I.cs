using UnityEngine;

/// <summary>
/// 触れると経験値を加算して消えるアイテム
/// アイテムオブジェクトにアタッチ
/// Collider の IsTrigger を ON にすること
/// </summary>
public class ExpItem : MonoBehaviour
{
    public int expAmount = 10; // 取得時の経験値

    void OnTriggerEnter(Collider other)
    {
        PlayerLevel playerLevel = other.GetComponentInParent<PlayerLevel>();
        if (playerLevel == null) return;

        playerLevel.AddExp(expAmount);
        Destroy(gameObject);
    }
}
