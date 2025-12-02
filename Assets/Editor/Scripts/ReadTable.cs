using System;
using System.IO;
using System.Reflection;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using OfficeOpenXml;

[InitializeOnLoad]
public class Startup
{
    // 这个方法会在运行前执行
    static Startup()
    {
        // 只在编辑器启动时执行一次
        if (EditorApplication.timeSinceStartup < 30) // 启动后30秒内
        {
            EditorApplication.delayCall += () =>
            {
                ExcelTableReader.ReadAllTables();
            };
        }
    }
}

public static class ExcelTableReader
{
    private static readonly string EXCEL_FOLDER_PATH = "Assets/Editor/Excel/";
    private static readonly string OUTPUT_FOLDER_PATH = "Assets/Resources/Data/";
    
    // 所有需要读取的Excel表配置
    private static readonly List<ExcelTableConfig> tableConfigs = new List<ExcelTableConfig>
    {
        new ExcelTableConfig("PackageData.xlsx", "PackageTable", typeof(PackageTable)),
    };
    

    [MenuItem("Tools/Excel/Read All Tables")]
    public static void ReadAllTables()
    {
        Debug.Log("开始读取所有Excel表格...");
        
        // 确保输出目录存在
        EnsureDirectoryExists(OUTPUT_FOLDER_PATH);
        
        bool allSuccess = true;
        
        foreach (var config in tableConfigs)
        {
            try
            {
                if (ReadTable(config))
                {
                    Debug.Log($"? {config.assetName} 读取成功");
                }
                else
                {
                    Debug.LogError($"? {config.assetName} 读取失败");
                    allSuccess = false;
                }
            }
            catch (Exception e)
            {
                Debug.LogError($"读取表格 {config.assetName} 时发生错误: {e.Message}");
                allSuccess = false;
            }
        }
        
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
        
        if (allSuccess)
        {
            Debug.Log("所有表格读取完成！");
        }
    }

    public static bool ReadTable(ExcelTableConfig config)
    {
        string excelPath = Path.Combine(EXCEL_FOLDER_PATH, config.excelFileName);
        
        if (!File.Exists(excelPath))
        {
            Debug.LogError($"Excel文件不存在: {excelPath}");
            return false;
        }
        
        FileInfo fileInfo = new FileInfo(excelPath);
        
        try
        {
            using (ExcelPackage excelPackage = new ExcelPackage(fileInfo))
            {
                ExcelWorksheet worksheet = excelPackage.Workbook.Worksheets[config.sheetName];
                
                if (worksheet == null)
                {
                    Debug.LogError($"工作表 {config.sheetName} 不存在");
                    return false;
                }
                
                // 创建ScriptableObject实例
                ScriptableObject table = ScriptableObject.CreateInstance(config.tableType);
                
                // 获取DataList字段
                FieldInfo dataListField = config.tableType.GetField("DataList", 
                    BindingFlags.Public | BindingFlags.Instance);
                
                if (dataListField == null)
                {
                    Debug.LogError($"在 {config.tableType.Name} 中找不到 DataList 字段");
                    return false;
                }
                
                // 获取DataList的值
                object dataList = dataListField.GetValue(table);
                
                // 获取DataList的Add方法
                MethodInfo addMethod = dataList.GetType().GetMethod("Add");
                
                // 获取列表元素的类型
                Type itemType = dataList.GetType().GetGenericArguments()[0];
                
                // 读取表头（假设第2行是字段名）
                Dictionary<int, FieldInfo> columnFieldMap = new Dictionary<int, FieldInfo>();
                int headerRow = 2; // 表头在第2行
                
                for (int col = worksheet.Dimension.Start.Column; col <= worksheet.Dimension.End.Column; col++)
                {
                    string fieldName = worksheet.GetValue(headerRow, col)?.ToString();
                    if (!string.IsNullOrEmpty(fieldName))
                    {
                        FieldInfo field = itemType.GetField(fieldName);
                        if (field != null)
                        {
                            columnFieldMap[col] = field;
                        }
                        else
                        {
                            Debug.LogWarning($"字段 {fieldName} 在 {itemType.Name} 中不存在");
                        }
                    }
                }
                
                // 读取数据（从第3行开始）
                int dataStartRow = 3;
                int rowCount = 0;
                
                for (int row = dataStartRow; row <= worksheet.Dimension.End.Row; row++)
                {
                    // 检查第一列是否为空，如果为空则跳过
                    object firstCellValue = worksheet.GetValue(row, worksheet.Dimension.Start.Column);
                    if (firstCellValue == null || string.IsNullOrWhiteSpace(firstCellValue.ToString()))
                    {
                        continue;
                    }
                    
                    // 创建新的数据项
                    object tableItem = Activator.CreateInstance(itemType);
                    
                    bool hasData = false;
                    
                    foreach (var kvp in columnFieldMap)
                    {
                        int col = kvp.Key;
                        FieldInfo field = kvp.Value;
                        
                        object cellValue = worksheet.GetValue(row, col);
                        
                        if (cellValue != null)
                        {
                            try
                            {
                                // 转换数据类型
                                object convertedValue = ConvertValue(cellValue, field.FieldType);
                                field.SetValue(tableItem, convertedValue);
                                hasData = true;
                            }
                            catch (Exception e)
                            {
                                Debug.LogError($"转换数据失败 (行{row}, 列{col}, 字段{field.Name}): {e.Message}");
                            }
                        }
                    }
                    
                    if (hasData)
                    {
                        addMethod.Invoke(dataList, new object[] { tableItem });
                        rowCount++;
                    }
                }
                
                Debug.Log($"读取到 {rowCount} 行数据");
                
                // 保存为asset文件
                string assetPath = Path.Combine(OUTPUT_FOLDER_PATH, config.assetName + ".asset");
                
                // 如果文件已存在，删除旧文件
                if (File.Exists(assetPath))
                {
                    AssetDatabase.DeleteAsset(assetPath);
                }
                
                AssetDatabase.CreateAsset(table, assetPath);
                EditorUtility.SetDirty(table);
                
                return true;
            }
        }
        catch (Exception e)
        {
            Debug.LogError($"读取Excel文件失败: {e.Message}");
            return false;
        }
    }

     private static object ConvertValue(object value, Type targetType)
    {
        if (value == null)
            return GetDefaultValue(targetType);
        
        if (targetType.IsEnum)
        {
            return Enum.Parse(targetType, value.ToString());
        }
        
        // 处理可空类型
        if (targetType.IsGenericType && targetType.GetGenericTypeDefinition() == typeof(Nullable<>))
        {
            Type underlyingType = Nullable.GetUnderlyingType(targetType);
            if (string.IsNullOrWhiteSpace(value.ToString()))
                return null;
            
            return Convert.ChangeType(value, underlyingType);
        }
        
        // 特殊处理int的转换
        if (targetType == typeof(int))
        {
            if (value is double)
                return Convert.ToInt32(value);
        }
        
        return Convert.ChangeType(value, targetType);
    }
    
    private static object GetDefaultValue(Type type)
    {
        return type.IsValueType ? Activator.CreateInstance(type) : null;
    }
    
    private static void EnsureDirectoryExists(string path)
    {
        if (!Directory.Exists(path))
        {
            Directory.CreateDirectory(path);
            AssetDatabase.Refresh();
        }
    }
}