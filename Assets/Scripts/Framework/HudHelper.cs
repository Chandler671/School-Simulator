using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using DG.Tweening;

public static class HudHelper
{
    // UI 淡入效果
    public static void FadeIn(this Canvas canvas, GameObject target, TweenCallback action)
    {
        var cg = target.GetOrAddComponent<CanvasGroup>();
        cg.alpha = 0;
        cg.DOFade(1, .3f).OnComplete(action);
    }

    // UI 淡出效果
    public static void FadeOut(this Canvas canvas, GameObject target, TweenCallback action)
    {
        var cg = target.GetOrAddComponent<CanvasGroup>();
        cg.DOFade(0, .3f).OnComplete(action);
    }

    // 发送游戏事件
    public static void SendEvent<T>(this Canvas canvas, T e) where T : GameEvent => 
        EventManager.QueueEvent(e);

    // 添加事件监听器
    public static void AddListener<T>(this Canvas canvas, EventManager.EventDelegate<T> del) where T : GameEvent => 
        EventManager.AddListener(del);

    // 移除事件监听器
    public static void RemoveListener<T>(this Canvas canvas, EventManager.EventDelegate<T> del) where T : GameEvent => 
        EventManager.RemoveListener(del);

    // 为按钮列表添加点击监听器
    public static void ButtonListAddListener(this Canvas canvas, List<Button> buttons, UnityAction action)
    {
        foreach (var bt in buttons)
        {
            bt.onClick.AddListener(action);
        }
    }

    // 移除按钮列表的所有点击监听器
    public static void ButtonListRemoveListener(this Canvas canvas, List<Button> buttons)
    {
        foreach (var bt in buttons)
        {
            bt.onClick.RemoveAllListeners();
        }
    }
}