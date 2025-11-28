using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class GMCmd : MonoBehaviour
{
    [MenuItem("GMCmd/打开背包主界面")]
    public static void OpenPackagePanel()
    {
        UIManager.Instance.OpenPanel(UIConst.PackagePanel);
    }

    [MenuItem("GMCmd/打开制作武器主界面")]
    public static void OpenProducePanel()
    {
        UIManager.Instance.OpenPanel(UIConst.ProducePanel);
    }


    [MenuItem("GMCmd/创建背包测试数据")]
    public static void CreateLocalPackageData()
    {
        // 保存数据
        PackageLocalData.Instance.items = new List<PackageLocalItem>();

        for (int i = 1; i < 9; i++)
        {
            if (i == 1)
            {
                PackageLocalItem packageLocalItem = new()
                {
                    uid = Guid.NewGuid().ToString(),
                    id = i,
                    num = 2,
                    putNum = 0
                };
                PackageLocalData.Instance.items.Add(packageLocalItem);
            }
            else if (i == 2)
            {
                PackageLocalItem packageLocalItem = new()
                {
                    uid = Guid.NewGuid().ToString(),
                    id = i,
                    num = 2,
                    putNum = 0
                };
                PackageLocalData.Instance.items.Add(packageLocalItem);
            }
            else if (i == 3)
            {
                PackageLocalItem packageLocalItem = new()
                {
                    uid = Guid.NewGuid().ToString(),
                    id = i,
                    num = 45,
                    putNum = 0
                };
                PackageLocalData.Instance.items.Add(packageLocalItem);
            }
            else if (i == 4)
            {
                PackageLocalItem packageLocalItem = new()
                {
                    uid = Guid.NewGuid().ToString(),
                    id = i,
                    num = 20,
                    putNum = 0
                };
                PackageLocalData.Instance.items.Add(packageLocalItem);
            }
            else if (i == 5)
            {
                PackageLocalItem packageLocalItem = new()
                {
                    uid = Guid.NewGuid().ToString(),
                    id = i,
                    num = 40,
                    putNum = 0
                };
                PackageLocalData.Instance.items.Add(packageLocalItem);
            }
            else if (i == 6)
            {
                PackageLocalItem packageLocalItem = new()
                {
                    uid = Guid.NewGuid().ToString(),
                    id = i,
                    num = 20,
                    putNum = 0
                };
                PackageLocalData.Instance.items.Add(packageLocalItem);
            }
            else if (i == 7)
            {
                PackageLocalItem packageLocalItem = new()
                {
                    uid = Guid.NewGuid().ToString(),
                    id = i,
                    num = 20,
                    putNum = 0
                };
                PackageLocalData.Instance.items.Add(packageLocalItem);
            }
            else if (i == 8)
            {
                PackageLocalItem packageLocalItem = new()
                {
                    uid = Guid.NewGuid().ToString(),
                    id = i,
                    num = 60,
                    putNum = 0
                };
                PackageLocalData.Instance.items.Add(packageLocalItem);
            }
        }
        PackageLocalData.Instance.SavePackage();


    }

    [MenuItem("GMCmd/读取背包测试数据")]
    public static void ReadLocalPackageData()
    {
        // 读取数据
        List<PackageLocalItem> readItems = PackageLocalData.Instance.LoadPackage();
        foreach (PackageLocalItem item in readItems)
        {
            Debug.Log(item);
        }
    }
}
