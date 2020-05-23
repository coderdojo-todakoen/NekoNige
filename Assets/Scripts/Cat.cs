using UnityEngine;

public class Cat : MonoBehaviour
{
    // 移動する速さ
    public float speed = 2;

    // Animatorコンポーネントを取得します
    private Animator animator;

    // 移動が開始された(何かキーが押された)かどうか
    private bool start;

    // 壁にぶつかり向きを変えている間trueにします
    private bool turning;

    // 向きを変える際の中心の座標
    private Vector3 center;
    // 向きを変える際の中心軸
    private Vector3 axis;
    // 向きを変える際の経過時間
    private float time;

    // Start is called before the first frame update
    void Start()
    {
        // Animatorコンポーネントを取得します
        animator = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        // 何かキーが押されたらスタートします
        if (!start)
        {
            // まだスタートしていない時に、何かキーが
            // 押されたら
            if (Input.anyKeyDown)
            {
                // 歩くアニメーションを開始して
                // スタートします
                animator.SetBool("walk", true);
                start = true;
            }
            return;
        }

        if (animator.GetBool("walk"))
        {
            // 歩くアニメーション中は移動します
            Move();
        }
    }

    // ネコの移動をおこないます
    // 壁にぶつかると向きを変えながら前へ進みます
    private void Move()
    {
        // 壁にぶつかり向きを変えている途中かどうかをチェックします
        if (!turning)
        {
            // 壁にぶつかっていなければ、前に移動します
            transform.Translate(Vector3.forward * Time.deltaTime * speed);
        }
        else
        {
            // 向きを変えている場合は、経過時間をチェックします
            if (time <= 1)
            {
                // 中心点、中心軸の周りを1秒間で90度回転します
                transform.RotateAround(center, axis, 90 * Time.deltaTime);
                time += Time.deltaTime;
            }
            else
            {
                // 1秒以上経過したら向きを変えるのを停めます
                turning = false;
            }
        }
    }

    // 右周りに向きを変えるように回転軸を設定します
    private void PrepareTurnRight()
    {
        axis = Vector3.up;
    }

    // 左周りに向きを変えるように回転軸を設定します
    private void PrepareTurnLeft()
    {
        axis = Vector3.down;
    }

    // 壁にぶつかったため、Update()時に向きを変えるよう設定します
    private void PrepareTurn(bool right)
    {
        if (turning)
        {
            // 向きを変えている間は、さらに壁にぶつかっても
            // 何もしません。(現在の向きを変える処理を続けます)
            return;
        }

        if (right)
        {
            // 右周りに向きを変えるように回転軸を設定します
            PrepareTurnRight();
        }
        else
        {
            // 左周りに向きを変えるように回転軸を設定します
            PrepareTurnLeft();
        }
        // 向きを変える際の中心座標を設定します
        // ネコの現在の中心座標をワールド座標に変換したものです
        center = transform.TransformPoint(Vector3.zero);
        // 向きを変える際の経過時間を設定します
        time = 0;
        // 次のUpdate()から向きを変えるようにします
        turning = true;
    }

    // 衝突を検出した場合に呼び出されます
    void OnTriggerEnter(Collider other)
    {
        switch (other.gameObject.name)
        {
            case "NorthWall":
                // 壁にぶつかった場合、向きを変えるようにします
                PrepareTurn(transform.forward.x > 0);
                break;
            case "SouthWall":
                // 壁にぶつかった場合、向きを変えるようにします
                PrepareTurn(transform.forward.x < 0);
                break;
            case "EastWall":
                // 壁にぶつかった場合、向きを変えるようにします
                PrepareTurn(transform.forward.z < 0);
                break;
            case "WestWall":
                // 壁にぶつかった場合、向きを変えるようにします
                PrepareTurn(transform.forward.z > 0);
                break;
            case "Mouse":
                // ネズミにぶつかった場合、移動を停めて
                // 次のアニメーションを開始します
                animator.SetBool("walk", false);

                // 効果音を再生します
                GetComponent<AudioSource>().PlayDelayed(0.6F);
                break;
            default:
                return;
        }
    }
}
