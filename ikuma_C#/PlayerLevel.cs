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
    public int jetItemUnlockLevel     = 3; // ジェットアイテム出現
    public int shapeChangeUnlockLevel = 4; // 形状変化解放
    public int gravityUnlockLevel     = 5; // 重力変化解放

    [Header("解放状況（確認用）")]
    public bool canJumpDash      = false;
    public bool canChangeShape   = false;
    public bool canJetItem       = false;
    public bool canChangeGravity = false;

    [Header("Lv4で表示されるジェットアイテム")]
    public GameObject jetItem;

    public PlayerLevelUI playerLevelUI;

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
        expToNext = (level * 2 - 1 ) * 10;
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
            playerLevelUI.ShowUnlockMessage("Spaceキーを押せ。Wキーで急げ。");
        }

        // Lv4：形状変化解放
        if (level >= shapeChangeUnlockLevel && !canChangeShape)
        {
            canChangeShape = true;
            if (_sc != null){
                _sc.isUnlocked = true;
            }

            playerLevelUI.ShowUnlockMessage(
            "Eキーを押せ"
            );
        }

        // Lv3：ジェットアイテム出現
        if (level >= jetItemUnlockLevel && !canJetItem)
        {
            canJetItem = true;
            if (jetItem != null)
            {
                jetItem.SetActive(true);
            }
            playerLevelUI.ShowUnlockMessage("ジェット燃料を取得してShiftキーを押せ。\n ");
        }

        // Lv5：重力変化解放（平面・球面どちらも）
        if (level >= gravityUnlockLevel && !canChangeGravity)
        {
            canChangeGravity = true;
            if (_gf != null)
                _gf.isUnlocked = true;
            if (_gs != null)
                _gs.isUnlocked = true;

            playerLevelUI.ShowUnlockMessage(
            "目の前の壁に触れろ。 "
            );
        }
    }
}