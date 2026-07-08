using UnityEngine;

/// <summary>
/// プレイヤーのレベル・経験値を管理する
/// Player にアタッチ
/// </summary>
public class PlayerLevel : MonoBehaviour
{
    [Header("レベル設定")]
    public int level        = 1;    // 現在のレベル
    public int exp          = 0;    // 現在の経験値
    public int expToNext    = 100;  // 次のレベルまでの経験値

    [Header("機能解放レベル")]
    public int gravityUnlockLevel = 2; // このレベルになると重力変化が使える

    [Header("解放状況（確認用）")]
    public bool canChangeGravity = false; // 重力変化が使えるか

    // 経験値を加算してレベルアップ判定（アイテムから呼ばれる）
    public void AddExp(int amount)
    {
        exp += amount;
        Debug.Log($"経験値 +{amount}　合計:{exp} / {expToNext}");

        // 経験値が上限を超えたらレベルアップ
        if (exp >= expToNext)
        {
            exp -= expToNext;
            LevelUp();
        }
    }

    void LevelUp()
    {
        level++;
        expToNext = level * 100; // レベルが上がるほど必要経験値が増える
        Debug.Log($"レベルアップ！ Lv{level}");

        // 機能の解放チェック
        CheckUnlocks();
    }

    void CheckUnlocks()
    {
        // 重力変化の解放
        if (level >= gravityUnlockLevel && !canChangeGravity)
        {
            canChangeGravity = true;
            Debug.Log("重力変化が解放されました！");
        }
    }
}
