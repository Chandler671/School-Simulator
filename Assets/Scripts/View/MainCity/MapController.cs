using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using System;
using Unity.VisualScripting;
using UnityEngine.EventSystems;
using Coffee.UIEffects;

/// <summary>
/// UI地图拖拽控制器
/// 为UI地图元素添加右键拖拽功能，并限制边界
/// </summary>
public class MapController : HudBase
{
    [Header("拖拽设置")]
    [SerializeField] private bool enableDrag = true;
    [SerializeField] private DragMode dragMode = DragMode.Map;
    [SerializeField][Range(0.1f, 1.9f)] private float dragSpeed;

    private Dragable dragableComponent;
    private MainCityLayer mainCityLayer;

    protected override void InitState()
    {
        mainCityLayer = basePanel as MainCityLayer;
        dragableComponent = Root.transform.Find("XGMap").gameObject.GetComponent<Dragable>();
    }

    protected override void AddListeners()
    {
        base.AddListeners();
        canvas.AddDragListener(dragableComponent, dragMode, onComplete, dragSpeed);
    }

    protected override void RemoveListeners()
    {
        base.RemoveListeners();
        if (dragableComponent == null) return;
        canvas.RemoveDragListener(dragableComponent);
    }

    private void onComplete()
    {
        
    }

}