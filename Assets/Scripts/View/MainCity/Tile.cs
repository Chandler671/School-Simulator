using UnityEngine;
using UnityEngine.EventSystems;

public class Tile : MonoBehaviour, IPointerClickHandler
{
    public int row;           // 行号
    public int col;           // 列号
    public bool isWalkable = true;   // 是否可行走

    private XGMap xGMap;

    void Start()
    {
        xGMap = FindObjectOfType<XGMap>();
        if (xGMap != null)
            xGMap.RegisterTile(this);
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (xGMap != null)
            xGMap.OnTileClicked(this);

        Debug.Log($"当前点击Grid坐标：{row}行、{col}列");
    }
}