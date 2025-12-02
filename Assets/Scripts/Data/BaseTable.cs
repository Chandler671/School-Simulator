using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// 所有表类的基类
public abstract class BaseTable<T> : ScriptableObject where T : new()
{
    public List<T> DataList = new List<T>();
}
