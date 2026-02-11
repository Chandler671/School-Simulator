using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

public static class CanvasExtension
{
    // 添加触发器监听器
    public static void AddTriggerListener(this Canvas canvas, Component obj, EventTriggerType type, UnityAction<BaseEventData> action)
    {
        // 先看有没有对应组件，没有就加上
        var trigger = obj.gameObject.GetOrAddComponent<EventTrigger>();
    
        // 再看看触发列表中有没有事件，没有就新建一个列表
        if (trigger.triggers == null || trigger.triggers.Count == 0)
        {
            trigger.triggers = new List<EventTrigger.Entry>();
        }
    
        // 再看事件列表中是不是已经存在对应类型的值，如果存在的话简单直接给那个事件加个侦听就好
        foreach (var e in trigger.triggers)
        {
            if (e.eventID == type)
            {
                e.callback.AddListener(action);
                return;
            }
        }
    
        // 到这里就是很遗憾没有对应类型的事件，那就实例化一个新的
        // 注意实例化完了以后还要把对应的事件类型和回调设定进去
        var entry = new EventTrigger.Entry();
        entry.eventID = type;
        entry.callback.AddListener(action);
    
        // 全部设定好了再加进去，要不然没有效果知道么
        trigger.triggers.Add(entry);
    }

    // 移除指定类型的触发器监听器
    public static void RemoveTriggerListener(this Canvas canvas, Component obj, EventTriggerType type, UnityAction<BaseEventData> action)
    {
        var trigger = obj.gameObject.GetComponent<EventTrigger>();
        if (trigger == null || trigger.triggers == null)
        {
            return;
        }
        
        foreach (var entry in trigger.triggers)
        {
            if (entry.eventID == type)
            {
                entry.callback.RemoveListener(action);
                
                // 如果没有监听器了，移除这个条目
                if (entry.callback.GetPersistentEventCount() == 0)
                {
                    trigger.triggers.Remove(entry);
                }
                break;
            }
        }
    }

    // 移除所有触发器监听器
    public static void RemoveAllTriggerListeners(this Canvas canvas, Component obj)
    {
        var trigger = obj.gameObject.GetComponent<EventTrigger>();
        if (trigger != null)
        {
            trigger.triggers?.Clear();
        }
    }

    // 添加拖拽监听器
    public static void AddDragListener(this Canvas canvas, Component obj, DragMode mode, UnityAction complete, float speed = 1f)
    {
        var dragable = obj.gameObject.GetOrAddComponent<Dragable>();
        dragable.Init(mode, speed);

        canvas.AddTriggerListener(obj, EventTriggerType.BeginDrag, dragable.OnBeginDrag);
        canvas.AddTriggerListener(obj, EventTriggerType.Drag, dragable.OnDrag);
        canvas.AddTriggerListener(obj, EventTriggerType.EndDrag, (x) => dragable.OnEndDrag(x, complete));
    }

    // 移除拖拽监听器
    public static void RemoveDragListener(this Canvas canvas, Component obj)
    {
        canvas.RemoveAllTriggerListeners(obj);
    }
}
