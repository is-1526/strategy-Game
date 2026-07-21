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
    public int expToNext = 10; // Lv1→Lv2 は10（テスト用）

    [Header("機能解放レベル")]
    public int jumpDashUnlockLevel    = 2; // ジャンプ・ダッシュ解放
    public int shapeChangeUnlockLevel = 3; // 形状変化解放
    public int jetItemUnlockLevel     = 4; // ジェットアイテム出現
    public int gravityUnlockLevel     = 5; // 重力変化解放

    [Header("解放状況（確認用）")]
    public bool canJumpDash      = false;
    public bool canChangeShape   = false;
    public bool canJetItem       = false;
    public bool canChangeGravity = false;

    [Header("Lv4で表示されるジェットアイテム")]
    public GameObject jetItem;

    private PlayerController _pc;
    private GravityFlat      _gf;
    private GravitySphere    _gs;
    private ShapeChanger     _sc;

    void Start()
    {
        _pc = GetComponent<PlayerController>();
        _gf = GetComponent<GravityFlat>();
        _gs = GetComponent<GravitySphere>();
        _sc = GetComponent<ShapeChanger>();

        // ジェットアイテムを非表示
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
        expToNext = level * 10;
        Debug.Log($"レベルアップ！ Lv{level}");
        CheckUnlocks();
    }

    void CheckUnlocks()
    {
        // Lv2：ジャンプ・ダッシュ解放
        if (level >= jumpDashUnlockLevel && !canJumpDash)
        {
            canJumpDash = true;
            if (_pc != null)
            {
                _pc.canJump = true;
                _pc.canDash = true;
            }
            Debug.Log("ジャンプ・ダッシュが解放されました！");
        }

        // Lv3：形状変化解放
        if (level >= shapeChangeUnlockLevel && !canChangeShape)
        {
            canChangeShape = true;
            if (_sc != null)
                _sc.isUnlocked = true;
            Debug.Log("形状変化が解放されました！");
        }

        // Lv4：ジェットアイテム出現
        if (level >= jetItemUnlockLevel && !canJetItem)
        {
            canJetItem = true;
            if (jetItem != null)
            {
                jetItem.SetActive(true);
                Debug.Log("ジェットアイテムが出現しました！");
            }
        }

        // Lv5：重力変化解放（平面・球面どちらも）
        if (level >= gravityUnlockLevel && !canChangeGravity)
        {
            canChangeGravity = true;
            if (_gf != null)
                _gf.isUnlocked = true;
            if (_gs != null)
                _gs.isUnlocked = true;
            Debug.Log("重力変化が解放されました！");
        }
    }
}