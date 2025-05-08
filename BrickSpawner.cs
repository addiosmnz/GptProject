using UnityEngine;

public class BrickSpawner : MonoBehaviour
{
    private GameConstants constants;
    public GameObject brickPrefab;
    public Vector2 spawnPosition = new Vector2(0, 4);

    private GameObject currentBrick; // 当前正在移动的砖块
    private bool hasDropped = true;  // 标记是否已掉落
    private CameraFollow cameraFollow;

    void Start()
    {
        constants = FindObjectOfType<GameConstants>();
        if (constants == null)//debug代码------------------可以去掉
        {
            Debug.LogError("未找到 GameConstants 实例，请检查场景中是否存在挂载该脚本的游戏对象！");
        }
        else
        {
            Debug.Log("成功获取 GameConstants 实例，spawnYOffset: " + constants.spawnYOffset);
        }//debug代码-----------------可以去掉
        cameraFollow = Camera.main.GetComponent<CameraFollow>();
        if (cameraFollow == null)
        {
            Debug.LogError("❌ CameraFollow 脚本没找到，请检查摄像机是否挂载并设置为 MainCamera！");
        }
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            // 🚫 正在等待砖块落地，不能继续按空格
            // if (isWaitingForSnap) return;

            if (hasDropped)
            {
                // ✅ 生成新砖块
                float spawnY = Camera.main.transform.position.y + constants.spawnYOffset;
                currentBrick = Instantiate(brickPrefab, new Vector3(0f, spawnY, 0f), Quaternion.identity);
                currentBrick.AddComponent<BrickMover>();
                currentBrick.AddComponent<BrickSnapper>();

                hasDropped = false;
            }
            else
            {
                // ✅ 触发下落，并标记“等待落地”
                currentBrick.GetComponent<BrickMover>().Drop();
                hasDropped = true;
            }
        }
    }
}