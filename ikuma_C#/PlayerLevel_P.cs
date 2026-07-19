using UnityEngine;

/// <summary>
/// 経験値・レベル・機能解放を管理する
/// Player にアタッチ
/// </summary>
public class PlayerLevel : MonoBehaviour
{
    [Header("レベル設定")]
    public int level     = 1;
    public int exp       = 0;
    public int expToNext = 100; // Lv1→Lv2 は100

    [Header("機能解放レベル")]
    public int jumpDashUnlockLevel   = 2; // ジャンプ・ダッシュ解放
    public int gravityUnlockLevel    = 3; // 重力変化解放

    [Header("レベルアップ時のジャンプ力増加")]
    public float jumpPowerBonus = 1f;

    [Header("解放状況（確認用）")]
    public bool canJumpDash      = false;
    public bool canChangeGravity = false;

    [Header("Lv2で表示されるジェットアイテム")]
    public GameObject jetItem; // シーンに配置したジェットアイテムをセット

    private PlayerController _pc;
    private GravityFlat      _gf;

    void Start()
    {
        _pc = GetComponent<PlayerController>();
        _gf = GetComponent<GravityFlat>();

        // 初期状態は重力変化無効（isUnlocked = false がデフォルト）
        // ジェットアイテムを非表示にする
        if (jetItem != null)
            jetItem.SetActive(false);
    }

    public void AddExp(int amount)
    {
        exp += amount;
        Debug.Log($"経験値 +{amount}　合計:{exp} / {expToNext}");

        while (exp >= expToNext)
        {
            exp -= expToNext;
            LevelUp();
        }
    }

    void LevelUp()
    {
        level++;
        expToNext = level * 100; // 次のレベルまでの経験値 = レベル × 100
        Debug.Log($"レベルアップ！ Lv{level}");

        // ジャンプ力増加
        if (_pc != null)
        {
            _pc.jumpPower += jumpPowerBonus;
            Debug.Log($"ジャンプ力UP → {_pc.jumpPower}");
        }

        CheckUnlocks();
    }

    void CheckUnlocks()
    {
        // Lv2：ジャンプ・ダッシュ解放 + ジェットアイテム出現
        if (level >= jumpDashUnlockLevel && !canJumpDash)
        {
            canJumpDash    = true;
            if (_pc != null)
            {
                _pc.canJump = true;
                _pc.canDash = true;
            }

            // ジェットアイテムを表示
            if (jetItem != null)
            {
                jetItem.SetActive(true);
                Debug.Log("ジェットアイテムが出現しました！");
            }

            Debug.Log("ジャンプ・ダッシュが解放されました！");
        }

        // Lv3：重力変化解放
        if (level >= gravityUnlockLevel && !canChangeGravity)
        {
            canChangeGravity = true;
            if (_gf != null)
                _gf.isUnlocked = true;
            Debug.Log("重力変化が解放されました！");
        }
    }
}