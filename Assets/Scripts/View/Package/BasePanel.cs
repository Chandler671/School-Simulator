using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class BasePanel : MonoBehaviour
{
    protected bool isRemove = false;
    protected new string name;

    protected virtual void Awake()
    {
        onInitCom();
    }

    void Start()
    {
        onOpenPanel();
        Refresh();
    }

    protected virtual void onInitCom()
    {

    }

    public virtual void Refresh()
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

    protected virtual void OnDestroy()
    {
        StopAllCoroutines();
        transform.DOKill();
    }
}

