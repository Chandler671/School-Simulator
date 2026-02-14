using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class XGMap : MonoBehaviour
{
    [Header("人物设置")]
    public RectTransform player;          // 人物对象
    private PlayerController playerController;

    private Dictionary<Vector2Int, Tile> tileDict = new Dictionary<Vector2Int, Tile>();
    private Vector2Int? currentPlayerTile; // 人物当前所在的格子坐标

    void Awake()
    {
        EventManager.AddListener<login_Success>(InitCom);
    }

    private void InitCom(login_Success d)
    {
        playerController = transform.Find("HeroNode").GetComponentInChildren<PlayerController>();
        currentPlayerTile = new Vector2Int(5, 9);
        Debug.Log("playerController组件初始化成功！！角色坐标初始成功！！");
    }

    // 格子注册
    public void RegisterTile(Tile tile)
    {
        tileDict[new Vector2Int(tile.row, tile.col)] = tile;
    }

    // 点击格子处理
    public void OnTileClicked(Tile clickedTile)
    {
        if (!clickedTile.isWalkable) return;

        if (!currentPlayerTile.HasValue)
        {
            SetPlayerPosition(clickedTile);
            return;
        }

        Vector2Int start = currentPlayerTile.Value;
        Vector2Int goal = new Vector2Int(clickedTile.row, clickedTile.col);

        // 如果点击的是当前格子，不移动
        if (start == goal) return;

        List<Vector2Int> path = FindPath(start, goal);
        if (path != null && path.Count > 0)
        {
            StopAllCoroutines();
            StartCoroutine(MovePlayerAlongPath(path));
        }
    }

    // ---------- A* 寻路核心 ----------
    // 节点定义（移到方法外部，避免内部类可能的作用域问题）
    private class AStarNode
    {
        public Vector2Int pos;
        public AStarNode parent;
        public int g; // 起点到当前代价
        public int h; // 启发式代价
        public int f => g + h;
    }

    private List<Vector2Int> FindPath(Vector2Int start, Vector2Int goal)
    {
        List<AStarNode> openList = new List<AStarNode>();
        HashSet<Vector2Int> closedSet = new HashSet<Vector2Int>();

        AStarNode startNode = new AStarNode { pos = start, g = 0, h = GetHeuristic(start, goal) };
        openList.Add(startNode);

        while (openList.Count > 0)
        {
            // 取 f 最小的节点
            openList.Sort((a, b) => a.f.CompareTo(b.f));
            AStarNode current = openList[0];
            openList.RemoveAt(0);
            closedSet.Add(current.pos);

            // 到达目标
            if (current.pos == goal)
            {
                return ReconstructPath(current);
            }

            // 四方向邻居
            Vector2Int[] dirs = { Vector2Int.up, Vector2Int.down, Vector2Int.left, Vector2Int.right };
            foreach (var dir in dirs)
            {
                Vector2Int neighborPos = current.pos + dir;

                // 检查格子是否存在且可行走
                if (tileDict.TryGetValue(neighborPos, out Tile neighborTile) && neighborTile.isWalkable)
                {
                    if (closedSet.Contains(neighborPos))
                        continue;

                    int newG = current.g + 1; // 相邻移动代价为1

                    AStarNode neighborNode = openList.Find(n => n.pos == neighborPos);
                    if (neighborNode == null)
                    {
                        neighborNode = new AStarNode
                        {
                            pos = neighborPos,
                            parent = current,
                            g = newG,
                            h = GetHeuristic(neighborPos, goal)
                        };
                        openList.Add(neighborNode);
                    }
                    else if (newG < neighborNode.g)
                    {
                        // 找到更优路径，更新
                        neighborNode.g = newG;
                        neighborNode.parent = current;
                    }
                }
            }
        }

        return null; // 无路径
    }

    // 重建路径
    private List<Vector2Int> ReconstructPath(AStarNode node)
    {
        List<Vector2Int> path = new List<Vector2Int>();
        while (node != null)
        {
            path.Add(node.pos);
            node = node.parent;
        }
        path.Reverse();
        return path;
    }

    // 曼哈顿距离
    private int GetHeuristic(Vector2Int a, Vector2Int b)
    {
        return Mathf.Abs(a.x - b.x) + Mathf.Abs(a.y - b.y);
    }

    // 沿路径移动
    private IEnumerator MovePlayerAlongPath(List<Vector2Int> path)
    {
        int startIndex = (path.Count > 0 && path[0] == currentPlayerTile.Value) ? 1 : 0;

        for (int i = startIndex; i < path.Count; i++)
        {
            Vector2Int tilePos = path[i];
            if (tileDict.TryGetValue(tilePos, out Tile tile))
            {
                Vector3 targetPos = tile.transform.position;
                Vector3 startPos = player.position;

                // 移动到目标格子
                float journeyLength = Vector3.Distance(startPos, targetPos);
                float startTime = Time.time;

                while (Vector3.Distance(player.position, targetPos) > 0.01f)
                {
                    // 计算当前帧的移动方向
                    Vector3 currentPos = player.position;
                    Vector3 direction = (targetPos - currentPos).normalized;

                    // 通知人物控制器：正在移动，并传入方向
                    if (playerController != null)
                        playerController.SetMoving(new Vector2(direction.x, direction.y));

                    // 移动人物
                    player.position = Vector3.MoveTowards(currentPos, targetPos, playerController.moveSpeed * Time.deltaTime);
                    yield return null;
                }

                // 到达格子后更新人物所在格子
                currentPlayerTile = tilePos;
            }
        }

        // 移动结束，停止动画
        if (playerController != null)
            playerController.StopMoving();
    }

    // 直接设置人物到指定格子（初始化）
    private void SetPlayerPosition(Tile tile)
    {
        currentPlayerTile = new Vector2Int(tile.row, tile.col);
        player.position = tile.transform.position;
    }

    private void ODestroy()
    {
        EventManager.RemoveListener<login_Success>(InitCom);
    }
}