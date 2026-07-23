using UnityEngine;
using TMPro;
using System.Collections;

/// <summary>
/// 特定タグのオブジェクトに触れると文字を表示する
/// プレイヤーにアタッチ
/// タグと表示文字列は Inspector で設定する
/// </summary>
public class TagMessageTrigger : MonoBehaviour
{
    [Header("タグと表示メッセージの設定")]
    public TagMessage[] tagMessages; // タグと文字列のペアを複数設定できる

    [Header("UI参照")]
    public TMP_Text messageText; // Canvas上のTextMeshProをセット

    [Header("表示時間（秒）")]
    public float displayDuration = 3f;

    void Start()
    {
        // 最初は非表示
        if (messageText != null)
            messageText.gameObject.SetActive(false);
    }

    void OnCollisionEnter(Collision collision)
    {
        CheckTag(collision.gameObject.tag);
    }

    void OnTriggerEnter(Collider other)
    {
        CheckTag(other.tag);
    }

    // タグを確認してメッセージを表示する
    void CheckTag(string tag)
    {
        foreach (TagMessage tm in tagMessages)
        {
            if (tm.tag == tag)
            {
                ShowMessage(tm.message);
                return;
            }
        }
    }

    void ShowMessage(string message)
    {
        StopAllCoroutines();
        StartCoroutine(DisplayMessage(message));
    }

    IEnumerator DisplayMessage(string message)
    {
        messageText.text = message;
        messageText.gameObject.SetActive(true);

        yield return new WaitForSeconds(displayDuration);

        messageText.gameObject.SetActive(false);
    }
}

/// <summary>
/// タグと表示メッセージのペア
/// Inspector で複数設定できる
/// </summary>
[System.Serializable]
public class TagMessage
{
    public string tag;     // 検知するタグ名
    public string message; // 表示するメッセージ
}
