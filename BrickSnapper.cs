using UnityEngine;

// 控制砖块落地吸附、削除、弹跳的逻辑组件
public class BrickSnapper : MonoBehaviour
{
    private GameConstants constants;
    private Rigidbody2D rb;              // 存储当前砖块的 Rigidbody2D，用于物理操作
    private bool snapped = false;        // 标志：砖块是否已落地吸附，防止重复处理
    void Start()
    {
        constants = FindObjectOfType<GameConstants>();
        // 在启动时获取 Rigidbody2D 组件引用
        rb = GetComponent<Rigidbody2D>();
    }

    // 当砖块发生碰撞时触发（落地或撞到其他砖）
    void OnCollisionEnter2D(Collision2D collision)
    {
        // 如果已经落地处理过，直接跳过
        if (snapped) return;

        // ✅ 第一块砖（只有一块砖 & 撞到了地面）→ 不做削砖逻辑
        if (collision.gameObject.CompareTag("Ground") &&
            GameObject.FindGameObjectsWithTag("Brick").Length == 1)
        {
            // 保持当前 X/Y 位置不变，只冻结物理状态 & 启动弹跳
            transform.rotation = Quaternion.identity;     // 重置角度，防止倾斜
            rb.bodyType = RigidbodyType2D.Static;         // 冻结刚体，砖块停止运动
            snapped = true;                               // 标记为已落地，避免重复触发
            StartCoroutine(BounceEffect());               // 启动弹跳反馈
            return;                                       // 不再执行后续逻辑
        }

        // ✅ 砖块撞到了其他砖或地面（正式处理）
        if (collision.gameObject.CompareTag("Ground") || collision.gameObject.CompareTag("Brick"))
        {
            float currentWidth = transform.localScale.x;  // 当前砖块的宽度1
            float offset = transform.position.x - collision.transform.position.x;  // 与目标物体的 X 轴偏移
            float absOffset = Mathf.Abs(offset);          // 偏移的绝对值
            float remainingWidth = currentWidth - absOffset;  // 可保留的砖块宽度

            Debug.Log("偏移量：" + absOffset + "，容差：" + constants.perfectAlignThreshold);

            // ✅ 情况 1：完美对齐（偏移极小）
            if (absOffset <= constants.perfectAlignThreshold)
            {
                // 计算撞击物体的顶部位置（中心 Y + 半高）
                float topY = collision.transform.position.y + collision.transform.localScale.y / 2f;
                float halfHeight = transform.localScale.y / 2.45f; // 当前砖块的半高

                // 精准对齐 X/Y，砖块稳稳落在目标物体正上方
                transform.position = new Vector3(
                    collision.transform.position.x,        // 吸附 X：目标中心
                    topY + halfHeight,                     // 吸附 Y：顶部 + 自身高度
                    transform.position.z
                );
            }
            // ❌ 情况 2：偏移太大 → 剩余宽度太小 → 砖块直接失败
            else if (remainingWidth <= constants.minKeepWidth)
            {
                Debug.Log("Game Over! Too far off.");      // 可用于 Game Over 提示
                Destroy(gameObject);                       // 销毁当前砖块
                return;
            }
            // ✂️ 情况 3：部分重叠 → 进行削砖逻辑
            else
            {
                float scaleRatio = remainingWidth / currentWidth; // 计算缩放比例（保留部分）

                // 缩放砖块的 X 尺寸（只缩宽度）
                Vector3 newScale = transform.localScale;
                newScale.x *= scaleRatio;
                // 计算撞击物体的顶部位置（中心 Y + 半高）  再次吸附-------------------------
                float topY = collision.transform.position.y + collision.transform.localScale.y / 2f;
                float halfHeight = transform.localScale.y / 2.45f;

                transform.position = new Vector3(
                    collision.transform.position.x,        // 吸附 X：目标中心
                    topY + halfHeight,                     // 吸附 Y：顶部 + 自身高度
                    transform.position.z
                );//再次吸附-------------------------
                transform.localScale = newScale;
                // 平均两砖中心点 → 重新设置砖块 X 坐标（视觉居中）
                float centerX = collision.transform.position.x + offset / 2f;
                Vector3 newPos = transform.position;
                newPos.x = centerX;
                transform.position = newPos;
            }

            // ✅ 所有情况的最后：执行落地逻辑

            transform.rotation = Quaternion.identity;     // 重置角度
            rb.bodyType = RigidbodyType2D.Static;         // 设置为静态 → 停止物理作用

            // 通知摄像机判断是否需要跟随向上
            CameraFollow cam = Camera.main.GetComponent<CameraFollow>();
            if (cam != null)
            {
                cam.MoveUpIfPastTriggerLine(transform);
            }

            snapped = true;                               // 标记为已落地
            StartCoroutine(BounceEffect());               // 弹跳反馈（无论是否削砖）
        }
    }

    // 弹跳效果协程，用于砖块落地后的视觉反馈
    private System.Collections.IEnumerator BounceEffect()
    {
        Debug.Log("🚀 弹跳协程启动！");

        Vector3 originalPos = transform.position;         // 记录初始位置（落地点）

        for (int i = 0; i < constants.bounceHeights.Length; i++)
        {
            float t = 0;
            float duration = constants.bounceTimes[i];    // 本次弹跳的持续时间
            float height = constants.bounceHeights[i];    // 本次弹跳的高度

            // 弹跳动画：从当前位置上下浮动（正弦曲线）
            while (t < duration)
            {
                t += Time.deltaTime;
                float percent = t / duration;
                float yOffset = Mathf.Sin(percent * Mathf.PI) * height;

                transform.position = new Vector3(
                    originalPos.x,
                    originalPos.y + yOffset,
                    originalPos.z
                );

                yield return null; // 等待下一帧
            }

            // 每次弹跳结束后 → 回到原始位置
            transform.position = originalPos;
        }

        // ⚠️ 建议：此处可以再加一行强制 Y 吸附，确保误差归零（可选）
        // transform.position = new Vector3(originalPos.x, snapTargetY, originalPos.z);
    }
}