using UnityEngine;

/// <summary>
/// プレイヤーの形状変化を管理する
/// Player にアタッチ
/// Lv3 で解放、E キーで立方体と直方体を切り替える
/// </summary>
public class ShapeChanger : MonoBehaviour
{
    [Header("形状オブジェクト")]
    public GameObject cubeShape;
    public GameObject rectangleShape;

    [Header("解放フラグ")]
    public bool isUnlocked = false;

    private bool _isCube = true;

    void Start()
    {
        // 初期状態：立方体を表示、直方体を非表示
        SetShape(cubeShape, true);
        SetShape(rectangleShape, false);
    }

    void Update()
    {
        if (!isUnlocked) return;

        if (Input.GetKeyDown(KeyCode.E))
            ToggleShape();
    }

    void ToggleShape()
    {
        _isCube = !_isCube;

        if (_isCube)
        {
            // 直方体の位置を引き継いで立方体を表示
            if (cubeShape != null && rectangleShape != null)
                cubeShape.transform.position = rectangleShape.transform.position;

            SetShape(cubeShape,      true);
            SetShape(rectangleShape, false);
            Debug.Log("形状変化：立方体");
        }
        else
        {
            // 立方体の位置を引き継いで直方体を表示
            if (cubeShape != null && rectangleShape != null)
                rectangleShape.transform.position = cubeShape.transform.position;

            SetShape(cubeShape,      false);
            SetShape(rectangleShape, true);
            Debug.Log("形状変化：直方体");
        }
    }

    // オブジェクトの表示・非表示とColliderの有効・無効を同時に切り替える
    void SetShape(GameObject shape, bool active)
    {
        if (shape == null) return;

        shape.SetActive(active);

        // Collider を明示的に有効・無効化
        Collider col = shape.GetComponent<Collider>();
        if (col != null)
            col.enabled = active;
    }
}