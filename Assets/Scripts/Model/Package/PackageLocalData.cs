using UnityEngine;
using System.Collections.Generic;

[System.Serializable]
public class PackageLocalData
{
    private static PackageLocalData _instance;

    public static PackageLocalData Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = new PackageLocalData();
            }
            return _instance;
        }

    }

    public List<PackageLocalItem> items;

    public void SavePackage()
    {
        string inventoryJson = JsonUtility.ToJson(this);
        Debug.Log($"保存背包数据: {inventoryJson}"); // 添加日志
        PlayerPrefs.SetString("PackageLocalData", inventoryJson);
        PlayerPrefs.Save();
    }

    // public List<PackageLocalItem> LoadPackage()
    // {
    //     if (items != null)
    //     {
    //         return items;
    //     }
    //     if (PlayerPrefs.HasKey("PackageLocalData"))
    //     {
    //         string inventoryJson = PlayerPrefs.GetString("PackageLocalData");
    //         PackageLocalData packageLocalData = JsonUtility.FromJson<PackageLocalData>(inventoryJson);
    //         items = packageLocalData.items;
    //         return items;
    //     }
    //     else
    //     {
    //         items = new List<PackageLocalItem>();
    //         return items;
    //     }
    // }
    // PackageLocalData.cs
    public List<PackageLocalItem> LoadPackage()
    {
        Debug.Log("开始加载背包数据...");

        if (items != null && items.Count > 0)
        {
            return items;
        }

        if (PlayerPrefs.HasKey("PackageLocalData"))
        {
            string inventoryJson = PlayerPrefs.GetString("PackageLocalData");
            Debug.Log($"从PlayerPrefs加载数据: {inventoryJson}");

            PackageLocalData packageLocalData = JsonUtility.FromJson<PackageLocalData>(inventoryJson);
            items = packageLocalData?.items ?? new List<PackageLocalItem>();
            return items;
        }
        else
        {
            Debug.LogWarning("未找到存储的背包数据，创建新列表");
            items = new List<PackageLocalItem>();
            return items;
        }
    }
}

[System.Serializable]
public class PackageLocalItem
{
    public string uid;
    public int id;
    public int num;
    public int putNum;

    public override string ToString()
    {
        return string.Format("[id]:{0} [num]:{1}", id, num);
    }

}