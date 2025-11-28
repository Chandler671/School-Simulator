using System.IO;
using System.Reflection;
using System;
using UnityEngine;
using UnityEditor;
using OfficeOpenXml;

[InitializeOnLoad]
public class Startup
{

    // 这个方法会在运行前执行
    static Startup()
    {
        string path = Application.dataPath + "/Editor/Excel/PackageData.xlsx";
        string assetName = "Package";

        FileInfo fileInfo = new FileInfo(path);

        // 创建序列化类
        PackageTable packageTable = (PackageTable)ScriptableObject.CreateInstance(typeof(PackageTable));
        using (ExcelPackage excelPackage = new ExcelPackage(fileInfo))
        {
            // 表格内的具体表单，这里是“Sheet1”
            ExcelWorksheet worksheet = excelPackage.Workbook.Worksheets["Sheet1"];
            
            for (int i = worksheet.Dimension.Start.Row + 2; i <= worksheet.Dimension.End.Row; i++)
            {
                PackageTableItem packageTableItem = new PackageTableItem();
                Type type = typeof(PackageTableItem);

                for (int j = worksheet.Dimension.Start.Column; j <= worksheet.Dimension.End.Column; j++)
                {
                    // 读取第i行第j列的数据
                    Debug.Log("数据内容：" + worksheet.GetValue(i, j).ToString());
                    
                    // 用反射的方法对packageTableItem进行赋值， worksheet.GetValue(1, j)对应表格第2行j列的数据
                    FieldInfo variable = type.GetField(worksheet.GetValue(2, j).ToString());
                    string tableValue = worksheet.GetValue(i, j).ToString();
                    variable.SetValue(packageTableItem, Convert.ChangeType(tableValue, variable.FieldType));
                }
                // 当前行赋值结束，添加到列表中
                packageTable.DataList.Add(packageTableItem);
            }
        }

        // 保存ScriptableObjects为.asset文件
        AssetDatabase.CreateAsset(packageTable, "Assets/Resources/" + assetName + ".asset");
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
    }
}
