using UnityEngine;

/// <summary>
/// 触れると経験値を加算して消えるアイテム
/// アイテムオブジェクトにアタッチ
/// Collider の IsTrigger を ON にすること
/// </summary>
public class ExpItem : MonoBehaviour
{
    public int expAmount = 100; // 取得時の経験値

    void OnTriggerEnter(Collider other)
    {
        PlayerLevel playerLevel = other.GetComponent<PlayerLevel>();
        if (playerLevel == null) return;

        playerLevel.AddExp(expAmount);
        Destroy(gameObject);
    }
}
