using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Mouse : MonoBehaviour
{
    // 移動する速さ
    public float speed = 10.0f;
    // 回転する速さ
    public float rotationSpeed = 100.0f;

    // キー入力で移動可能かどうか
    // ネコにぶつかった後、moveはfalseになります
    private bool move = true;
    // ネコにぶつかった際のネズミの位置
    private Vector3 fromPosition;
    // ネコにぶつかった後の移動先
    private Vector3 downPosition;
    // ネコにぶつかった後のカメラの方向
    private Vector3 lookPosition;
    // ネコにぶつかった後の経過時間
    private float time;

    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        if (move)
        {
            // キー入力による移動をおこないます
            Move();
        }
        else
        {
            // ネコにぶつかった後の移動をおこないます
            FallDown();
        }
    }

    // ネズミの移動をおこないます
    // キー入力に応じて、前後への移動と、左右への回転をおこないます
    private void Move()
    {
        // Get the horizontal and vertical axis.
        // By default they are mapped to the arrow keys.
        // The value is in the range -1 to 1
        float translation = Input.GetAxis("Vertical") * speed;
        float rotation = Input.GetAxis("Horizontal") * rotationSpeed;

        // Make it move 10 meters per second instead of 10 meters per frame...
        translation *= Time.deltaTime;
        rotation *= Time.deltaTime;

        // Move translation along the object's z-axis
        transform.Translate(0, 0, translation);

        // Rotate around our y-axis
        transform.Rotate(0, rotation, 0);
    }

    // ネコにぶつかった後の移動をおこないます
    private void FallDown()
    {
        if (time < 5)
        {
            // ネコの正面に移動します
            transform.position = Vector3.Slerp(fromPosition, downPosition, time);

            // ネコの方向を向きます
            transform.LookAt(lookPosition, Vector3.up);
        }
        else if (time < 7)
        {
            // 5秒経過後、7秒まで暗転します
            GameObject panel = GameObject.Find("Panel");
            Image image = panel.GetComponent<Image>();
            image.color = new Color(0, 0, 0, Mathf.Max(Mathf.Min((time - 5) / 2, 1), 0));
        }
        else if (time < 8)
        {
            // シーンを読み込み直します
            SceneManager.LoadScene("Main");
        }

        time += Time.deltaTime;
    }

    // ネコにぶつかった場合の処理をおこないます
    private void PrepareDown(GameObject cat)
    {
        // キー入力による移動を停めます
        move = false;

        // ぶつかった際のネズミの位置を取得します
        fromPosition = transform.position;

        // ぶつかった際のネコの位置を取得し、その正面の座標を移動先とします
        downPosition = cat.transform.position + cat.transform.forward;
        // ネコの下に位置するようにy座標を小さくします
        downPosition.y = -0.2F;

        // ぶつかった際のネコの位置を取得し、その正面の座標をカメラの方向とします
        lookPosition = cat.transform.position + cat.transform.forward * 0.2F;
        // ネコの顔に視線が向くように座標を調整します
        lookPosition.y = 0.5F;
    }

    // 衝突を検出した場合に呼び出されます
    void OnTriggerEnter(Collider other)
    {
        switch (other.gameObject.name)
        {
            case "Cat":
                // ネコにぶつかったので、移動を停めます
                PrepareDown(other.gameObject);
                break;
            default:
                break;
        }
    }
}
