using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Events;

public enum DragMode
{
    Map,
    Obj
}

public class Dragable : MonoBehaviour
{
    public DragMode DragMode = DragMode.Map;
    [Range(0.1f, 1.9f)]
    public float DragSpeed = 1f;

    public void Init(DragMode dragMode, float dragSpeed = 1f)
    {
        DragMode = dragMode;
        DragSpeed = dragSpeed > 1.9f ? 1.9f : dragSpeed < 0.1f ? 0.1f : dragSpeed;
    }

    Vector2 lastPos;
    RectTransform rt;
    Vector2 lastAnchorMin;
    Vector2 lastAnchorMax;

    public void OnBeginDrag(BaseEventData data)
    {
        // 将基类的Data转化为对应子类
        var d = data as PointerEventData;
        // 初始化屏幕位置
        lastPos = d.position;

        rt = GetComponent<RectTransform>();
        lastAnchorMin = rt.anchorMin;
        lastAnchorMax = rt.anchorMax;

        // 将锚框设置为四周扩展类型的预设，方便后续判断和屏幕边缘的距离
        rt.SetRtAnchorSafe(Vector2.zero, Vector2.one);
    }

    public void OnDrag(BaseEventData data)
    {
        var d = data as PointerEventData;
        // 一帧内拖动的向量
        Vector2 offse = d.position - lastPos;

        // 检测拖动的方向与边缘的关系
        if (CheckDragLimit(offse))
        {
            rt.anchoredPosition += offse * DragSpeed;

            // 极限快速拖动时单帧拖动距离超出范围的归位检测
            ResetRtOffset();
        }
        lastPos = d.position;
    }

    public void OnEndDrag(BaseEventData data, UnityAction complete)
    {
        // 还原拖动之前的预设
        rt.SetRtAnchorSafe(lastAnchorMin, lastAnchorMax);
        complete();
    }

    bool CheckDragLimit(Vector2 offse)
    {
        bool result = false;
        if (offse.x >= 0 && offse.y >= 0)
        {
            // 向右上拖动
            return DragMode == DragMode.Map ? rt.offsetMin.x < 0 && rt.offsetMin.y < 0 :
                rt.offsetMax.x < 0 && rt.offsetMax.y < 0;
        }
        else if (offse.x >= 0 && offse.y < 0)
        {
            // 向右下拖动
            return DragMode == DragMode.Map ? rt.offsetMin.x < 0 && rt.offsetMax.y > 0 :
                rt.offsetMax.x < 0 && rt.offsetMin.y > 0;
        }
        else if (offse.x < 0 && offse.y >= 0)
        {
            // 向左上拖动
            return DragMode == DragMode.Map ? rt.offsetMax.x > 0 && rt.offsetMin.y < 0 :
                rt.offsetMin.x > 0 && rt.offsetMax.y < 0;
        }
        else if (offse.x < 0 && offse.y < 0)
        {
            // 向左下拖动
            return DragMode == DragMode.Map ? rt.offsetMax.x > 0 && rt.offsetMax.y > 0 :
                rt.offsetMin.x > 0 && rt.offsetMin.y > 0;
        }
        return result;
    }

    void ResetRtOffset()
    {
        switch (DragMode)
        {
            case DragMode.Map:
                if (rt.offsetMin.x > 0)
                    rt.anchoredPosition -= new Vector2(rt.offsetMin.x, 0);

                if (rt.offsetMin.y > 0)
                    rt.anchoredPosition -= new Vector2(0, rt.offsetMin.y);

                if (rt.offsetMax.x < 0)
                    rt.anchoredPosition -= new Vector2(rt.offsetMax.x, 0);

                if (rt.offsetMax.y < 0)
                    rt.anchoredPosition -= new Vector2(0, rt.offsetMax.y);
                break;
            case DragMode.Obj:
                if (rt.offsetMin.x < 0)
                    rt.anchoredPosition -= new Vector2(rt.offsetMin.x, 0);

                if (rt.offsetMin.y < 0)
                    rt.anchoredPosition -= new Vector2(0, rt.offsetMin.y);

                if (rt.offsetMax.x > 0)
                    rt.anchoredPosition -= new Vector2(rt.offsetMax.x, 0);

                if (rt.offsetMax.y > 0)
                    rt.anchoredPosition -= new Vector2(0, rt.offsetMax.y);
                break;
        }
    }
}