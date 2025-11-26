using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class BasePanel : MonoBehaviour
{
    protected bool isRemove = false;
    protected new string name;

    void Awake()
    {
        
    }

    void Start()
    {
        onInitCom();
        onRefreshView();
    }

    protected virtual void onInitCom()
    {
        
    }

    protected virtual void onRefreshView()
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
}

