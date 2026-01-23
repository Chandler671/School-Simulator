using System;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// PlayerPrefs 批量操作管理器
/// 减少磁盘I/O，优化性能，避免频繁保存
/// </summary>
public class PlayerPrefsManager
{
    #region 单例模式
    private static PlayerPrefsManager _instance;
    public static PlayerPrefsManager Instance
    {
       get
        {
            if (_instance == null)
            {
                _instance = new PlayerPrefsManager();
            }
            return _instance;
        }
    }
    #endregion

    #region 内部数据结构
    // 数据缓存字典
    private Dictionary<string, object> _dataCache = new Dictionary<string, object>();
    
    // 脏标记：数据是否被修改但未保存
    public bool _isDirty = false;
    
    // 最大缓存条目数，防止内存占用过大
    private const int MAX_CACHE_SIZE = 100;

     // 自动保存数据间隔（秒）
    public float _autoSaveInterval = 30f;
    // 自动保存计时器
    public float _autoSaveTimer = 0f;
    #endregion

    private void OnApplicationPause(bool pauseStatus)
    {
        // 应用暂停时自动保存（移动端）
        if (pauseStatus && _isDirty)
        {
            Debug.Log("应用暂停，自动保存数据");
            ForceSave();
        }
    }

    private void OnApplicationQuit()
    {
        // 应用退出时自动保存
        if (_isDirty)
        {
            Debug.Log("应用退出，自动保存数据");
            ForceSave();
        }
    }

    #region 公共API - 设置数据
    /// <summary>
    /// 设置整数值（批量缓存）
    /// </summary>
    public void SetInt(string key, int value)
    {
        if (string.IsNullOrEmpty(key))
        {
            Debug.LogWarning("PlayerPrefs键不能为空");
            return;
        }

        // 检查缓存是否已满
        if (!_dataCache.ContainsKey(key) && _dataCache.Count >= MAX_CACHE_SIZE)
        {
            Debug.LogWarning("缓存已满，强制保存并清理");
            ForceSave();
        }

        // 检查值是否真的改变了
        int oldValue = GetInt(key);
        if (oldValue != value)
        {
            _dataCache[key] = value;
            _isDirty = true;
        }
    }

    /// <summary>
    /// 设置浮点数值
    /// </summary>
    public void SetFloat(string key, float value)
    {
        if (string.IsNullOrEmpty(key)) return;
        
        float oldValue = GetFloat(key);
        if (Mathf.Abs(oldValue - value) > 0.0001f) // 浮点数比较容差
        {
            _dataCache[key] = value;
            _isDirty = true;
        }
    }

    /// <summary>
    /// 设置字符串值
    /// </summary>
    public void SetString(string key, string value)
    {
        if (string.IsNullOrEmpty(key)) return;
        
        string oldValue = GetString(key);
        if (oldValue != value)
        {
            _dataCache[key] = value;
            _isDirty = true;
        }
    }

    /// <summary>
    /// 设置布尔值（转换为int存储）
    /// </summary>
    public void SetBool(string key, bool value)
    {
        SetInt(key, value ? 1 : 0);
    }

    /// <summary>
    /// 保存复杂对象（JSON序列化）
    /// </summary>
    public void SetObject<T>(string key, T obj) where T : class
    {
        try
        {
            string json = JsonUtility.ToJson(obj);
            SetString(key, json);
        }
        catch (Exception e)
        {
            Debug.LogError($"序列化对象失败: {e.Message}");
        }
    }
    #endregion

    #region 公共API - 获取数据
    /// <summary>
    /// 获取整数值（优先从缓存读取）
    /// </summary>
    public int GetInt(string key, int defaultValue = 0)
    {
        // 1. 从缓存读取
        if (_dataCache.ContainsKey(key) && _dataCache[key] is int cachedValue)
        {
            return cachedValue;
        }
        
        // 2. 从PlayerPrefs读取
        if (PlayerPrefs.HasKey(key))
        {
            int value = PlayerPrefs.GetInt(key, defaultValue);
            // 缓存读取的值
            _dataCache[key] = value;
            return value;
        }
        
        return defaultValue;
    }

    /// <summary>
    /// 获取浮点数值
    /// </summary>
    public float GetFloat(string key, float defaultValue = 0f)
    {
        if (_dataCache.ContainsKey(key) && _dataCache[key] is float cachedValue)
        {
            return cachedValue;
        }
        
        if (PlayerPrefs.HasKey(key))
        {
            float value = PlayerPrefs.GetFloat(key, defaultValue);
            _dataCache[key] = value;
            return value;
        }
        
        return defaultValue;
    }

    /// <summary>
    /// 获取字符串值
    /// </summary>
    public string GetString(string key, string defaultValue = "")
    {
        if (_dataCache.ContainsKey(key) && _dataCache[key] is string cachedValue)
        {
            return cachedValue;
        }
        
        if (PlayerPrefs.HasKey(key))
        {
            string value = PlayerPrefs.GetString(key, defaultValue);
            _dataCache[key] = value;
            return value;
        }
        
        return defaultValue;
    }

    /// <summary>
    /// 获取布尔值
    /// </summary>
    public bool GetBool(string key, bool defaultValue = false)
    {
        int intValue = GetInt(key, defaultValue ? 1 : 0);
        return intValue != 0;
    }

    /// <summary>
    /// 获取复杂对象
    /// </summary>
    public T GetObject<T>(string key, T defaultValue = null) where T : class
    {
        string json = GetString(key, string.Empty);
        if (!string.IsNullOrEmpty(json))
        {
            try
            {
                return JsonUtility.FromJson<T>(json);
            }
            catch (Exception e)
            {
                Debug.LogError($"反序列化对象失败: {e.Message}");
            }
        }
        return defaultValue;
    }
    #endregion

    #region 批量操作API
    /// <summary>
    /// 批量设置多个值（减少调用次数）
    /// </summary>
    public void SetMultiple(Dictionary<string, object> values)
    {
        foreach (var kvp in values)
        {
            switch (kvp.Value)
            {
                case int intVal: SetInt(kvp.Key, intVal); break;
                case float floatVal: SetFloat(kvp.Key, floatVal); break;
                case string stringVal: SetString(kvp.Key, stringVal); break;
                case bool boolVal: SetBool(kvp.Key, boolVal); break;
                default:
                    Debug.LogWarning($"不支持的类型: {kvp.Value.GetType()}");
                    break;
            }
        }
    }

    /// <summary>
    /// 强制立即保存所有缓存数据到磁盘
    /// </summary>
    public void ForceSave()
    {
        if (!_isDirty || _dataCache.Count == 0)
        {
            return;
        }

        int savedCount = 0;
        foreach (var kvp in _dataCache)
        {
            try
            {
                switch (kvp.Value)
                {
                    case int intVal:
                        PlayerPrefs.SetInt(kvp.Key, intVal);
                        break;
                    case float floatVal:
                        PlayerPrefs.SetFloat(kvp.Key, floatVal);
                        break;
                    case string stringVal:
                        PlayerPrefs.SetString(kvp.Key, stringVal);
                        break;
                }
                savedCount++;
            }
            catch (Exception e)
            {
                Debug.LogError($"保存键 '{kvp.Key}' 失败: {e.Message}");
            }
        }

        PlayerPrefs.Save();
        _dataCache.Clear();
        _isDirty = false;
        
        Debug.Log($"批量保存完成，共保存 {savedCount} 条数据");
    }

    /// <summary>
    /// 清空缓存（不保存）
    /// </summary>
    public void ClearCache()
    {
        _dataCache.Clear();
        _isDirty = false;
        Debug.Log("缓存已清空");
    }

    /// <summary>
    /// 删除指定键（缓存和持久化）
    /// </summary>
    public void DeleteKey(string key)
    {
        if (_dataCache.ContainsKey(key))
        {
            _dataCache.Remove(key);
        }
        PlayerPrefs.DeleteKey(key);
        _isDirty = true; // 标记需要保存
    }

    /// <summary>
    /// 检查键是否存在
    /// </summary>
    public bool HasKey(string key)
    {
        return _dataCache.ContainsKey(key) || PlayerPrefs.HasKey(key);
    }
    #endregion

    #region 调试与监控
    /// <summary>
    /// 获取当前缓存状态
    /// </summary>
    public void PrintCacheStatus()
    {
        Debug.Log($"=== PlayerPrefs缓存状态 ===");
        Debug.Log($"缓存条目数: {_dataCache.Count}");
        Debug.Log($"脏标记: {_isDirty}");
        Debug.Log($"自动保存倒计时: {_autoSaveInterval - _autoSaveTimer:F1}秒");
        
        foreach (var kvp in _dataCache)
        {
            Debug.Log($"  [{kvp.Key}] = {kvp.Value} ({kvp.Value.GetType().Name})");
        }
    }

    /// <summary>
    /// 设置自动保存间隔（秒，0表示禁用）
    /// </summary>
    public void SetAutoSaveInterval(float seconds)
    {
        _autoSaveInterval = Mathf.Max(0, seconds);
        if (_autoSaveInterval > 0 && _isDirty)
        {
            _autoSaveTimer = 0f; // 重置计时器
        }
    }
    #endregion
}