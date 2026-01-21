using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public abstract class BasePanel : MonoBehaviour
{
    protected bool isRemove = false;
    protected new string name;

    void Awake()
    {
        onInitCom();
    }

    void Start()
    {
        onOpenPanel();
        onRefreshView();
    }

    protected virtual void onInitCom()
    {

    }

    protected virtual void onRefreshView()
    {

    }

    protected virtual void onOpenPanel()
    {

    }

    public virtual void SetActive(bool active)
    {
        gameObject.SetActive(active);
    }

    public virtual void OpenPanel(string name)
    {
        this.name = name;
        SetActive(true);
    }

    public virtual void ClosePanel()
    {
        isRemove = true;
        SetActive(false);
        Destroy(gameObject);
        if (UIManager.Instance.panelDict.ContainsKey(name))
        {
            UIManager.Instance.panelDict.Remove(name);
        }
    }

    void OnDisable()
    {
        StopAllCoroutines();
        transform.DOKill();
    }
}

