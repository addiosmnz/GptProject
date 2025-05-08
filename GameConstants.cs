using UnityEngine;

// GameConstants 类用于集中管理游戏中所有可调的数值参数
// 将它挂载在场景中的一个 GameObject 上（如 GameManager），供其他脚本引用
public class GameConstants : MonoBehaviour
{
    // ===== BrickMover 相关参数 =====

    // 砖块左右移动的速度（单位：单位/秒）
    public float brickMoveSpeed = 2f;

    // 砖块左右移动的最大范围（从中心向左右偏移的最大距离）
    public float brickMoveRange = 3f;

    // ===== BrickSpawner 相关参数 =====

    // 新砖块生成时相对于当前最高砖块的垂直偏移距离
    // 例如设置为 4 表示新砖出现在上一砖上方 4 个单位处
    public float spawnYOffset = 4f;

    // ===== BrickSnapper 相关参数 =====

    // 砖块削除时保留的最小宽度（小于此值将判定失败，砖块销毁）
    public float minKeepWidth = 0.05f;

    // 用于判断“完美对齐”的水平偏移容差
    // 若新砖与下方砖的偏移小于此值，则视为完美对齐，不削除
    public float perfectAlignThreshold = 0.001f;

    // 弹跳动画的三个阶段的弹跳高度（单位：Y轴）
    // 玩家会看到砖块落下后轻微上下弹动，形成反馈感
    public float[] bounceHeights = { 0.3f, 0.2f, 0.1f };

    // 对应每个弹跳阶段的持续时间（单位：秒）
    public float[] bounceTimes = { 0.25f, 0.18f, 0.16f };

    // ===== CameraFollow 相关参数 =====

    // 摄像机跟随的平滑速度（越大越快）
    public float cameraFollowSpeed = 2f;

    // 摄像机触发向上移动的阈值线（相对于摄像机中心 Y 轴的偏移）
    // 当前砖块超过此线才触发镜头向上移动
    public float cameraTriggerLineOffset = -0.5f;
}
