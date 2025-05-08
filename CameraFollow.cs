using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    private GameConstants constants;
    public float followSpeed;
    public float triggerLineOffset; // 触发摄像机跟随的砖块顶部相对于摄像机位置的高度偏移  可修改此参数调整触发值
    private bool shouldMove = false;
    private float targetY;
    private float minY;

    void Start()
    {
        constants = FindObjectOfType<GameConstants>();
        followSpeed = constants.cameraFollowSpeed;
        triggerLineOffset = constants.cameraTriggerLineOffset;
        minY = transform.position.y;
    }

    void LateUpdate()
    {
        if (!shouldMove) return;

        float newY = Mathf.Lerp(transform.position.y, targetY, followSpeed * Time.deltaTime);
        transform.position = new Vector3(transform.position.x, newY, transform.position.z);

        if (Mathf.Abs(transform.position.y - targetY) < 0.01f)
        {
            shouldMove = false;
        }
    }

    public void MoveUpIfPastTriggerLine(Transform obj)
    {
        float triggerLineY = transform.position.y + triggerLineOffset;  // 触发线
        float objTopY = obj.position.y + GetObjectHeight(obj) / 2f;     // 砖块顶部

        if (objTopY > triggerLineY)
        {
            float shiftAmount = GetObjectHeight(obj);
            targetY = transform.position.y + shiftAmount;
            shouldMove = true;
        }
    }

    float GetObjectHeight(Transform obj)
    {
        Collider2D col = obj.GetComponent<Collider2D>();
        if (col != null)
        {
            return col.bounds.size.y;
        }
        return 1f; // 默认高度
    }
}