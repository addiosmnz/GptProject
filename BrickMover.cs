using UnityEngine;

public class BrickMover : MonoBehaviour
{
    private GameConstants constants;
    public float moveSpeed;     // 砖块移动的速度
    public float moveRange;     // 移动范围

    private bool isDropping = false;
    private float direction = 1f;
    private Rigidbody2D rb;

    void Start()
    {
        constants = FindObjectOfType<GameConstants>();
        moveSpeed = constants.brickMoveSpeed;
        moveRange = constants.brickMoveRange;

        // 添加刚体组件并设置为 Kinematic，一开始不运动
        rb = gameObject.AddComponent<Rigidbody2D>();
        rb.bodyType = RigidbodyType2D.Kinematic;
    }

    void Update()
    {
        if (isDropping) return;

        // 砖块移动逻辑
        float newX = transform.position.x + direction * moveSpeed * Time.deltaTime;

        if (Mathf.Abs(newX) > moveRange)
        {
            direction *= -1;
            newX = Mathf.Clamp(newX, -moveRange, moveRange);
        }

        transform.position = new Vector3(newX, transform.position.y, transform.position.z);
    }

    public void Drop()
    {
        isDropping = true;

        // 改为 Dynamic，开始受重力影响
        rb.bodyType = RigidbodyType2D.Dynamic;
        rb.gravityScale = 1;

        // 添加砖块吸附脚本
        gameObject.AddComponent<BrickSnapper>();
    }
}