using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;

public class PlayerLevelUI : MonoBehaviour
{
    public PlayerLevel playerLevel;

    [Header("レベルUI")]
    public TMP_Text levelText;
    public Slider expSlider;

    [Header("能力解放UI")]
    public TMP_Text unlockText;

    void Start()
    {
        unlockText.gameObject.SetActive(false);
    }

    void Update()
    {
        // レベル表示
        levelText.text = "Lv." + playerLevel.level;

        // 経験値ゲージ
        expSlider.value = (float)playerLevel.exp / playerLevel.expToNext;
    }

    // PlayerLevelから呼び出す
    public void ShowUnlockMessage(string message)
    {
        StopAllCoroutines();
        StartCoroutine(ShowMessage(message));
    }

    IEnumerator ShowMessage(string message)
    {
        unlockText.text = message;
        unlockText.gameObject.SetActive(true);

        yield return new WaitForSeconds(5f);

        unlockText.gameObject.SetActive(false);
    }
}