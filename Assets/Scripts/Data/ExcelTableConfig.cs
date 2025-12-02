[System.Serializable]
public class ExcelTableConfig
{
    public string excelFileName;      // Excel文件名
    public string sheetName = "Sheet1"; // 工作表名
    public string assetName;          // 生成的asset文件名
    public System.Type tableType;     // 对应的ScriptableObject类型
    
    public ExcelTableConfig(string excel, string asset, System.Type type)
    {
        excelFileName = excel;
        assetName = asset;
        tableType = type;
    }
}