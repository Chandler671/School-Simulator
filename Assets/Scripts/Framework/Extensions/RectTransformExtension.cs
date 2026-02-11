using UnityEngine;

public static class RectTransformExtensions
{
    /// <summary>
    /// 安全地设置RectTransform的锚点
    /// </summary>
    public static void SetRtAnchorSafe(this RectTransform rt, Vector2 anchorMin, Vector2 anchorMax)
    {
        // 保存当前位置和大小
        Vector2 savedPosition = rt.anchoredPosition;
        Vector2 savedSize = rt.sizeDelta;

        // 设置锚点
        rt.anchorMin = anchorMin;
        rt.anchorMax = anchorMax;

        // 恢复位置和大小
        rt.anchoredPosition = savedPosition;
        rt.sizeDelta = savedSize;
    }
}
