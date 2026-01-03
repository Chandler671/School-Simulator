using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


class CellDisplayData
{
    public PackageLocalItem localData;
    public int displayNum;
}

public class PackagePanel : BasePanel
{
    private Transform UIOwnNumber;
    private Transform UICloseBtn;
    private Transform UICenter;
    private Transform UIScrollView;
    public  GameObject UIDetailPanel;
    private Transform UIDetailPanelExitBtn;
    private Transform UIDetailName;
    private Transform UIDetailNum;
    private Transform UIDetailDescription;
    private Transform UIDetailIcon;
    public GameObject PackageUIItemPrefab;
    private string _chooseUid;
    public GameObject packageUICellPrefab;
    // 背包总格子数
    private const int remainingCell = 13;
    public string chooseUID
    {
        get
        {
            return _chooseUid;
        }
        set
        {
            _chooseUid = value;
            RefreshDetail();
        }
    }

    protected override void onInitCom()
    {
        UIOwnNumber = transform.Find("Top/OwnNumber");
        UICloseBtn = transform.Find("Right/ShutDown/Button");
        UICenter = transform.Find("Center");
        UIScrollView = UICenter.Find("ScrollView");
        UIDetailPanelExitBtn = UIDetailPanel.transform.Find("Top/ExitButton");
        UIDetailName = UIDetailPanel.transform.Find("Name");
        UIDetailNum = UIDetailPanel.transform.Find("Num");
        UIDetailDescription = UIDetailPanel.transform.Find("Description");
        UIDetailIcon = UIDetailPanel.transform.Find("Icon");
        
        UIDetailPanel.gameObject.SetActive(false);
        // 添加关闭按钮点击事件
        UICloseBtn.GetComponent<Button>().onClick.AddListener(OnClickCloseBtn);
        // 添加详情面板退出按钮点击事件
        UIDetailPanelExitBtn.GetComponent<Button>().onClick.AddListener(OnClickDetailExitBtn);
    }

    protected override void onRefreshView()
    {
        RefreshScroll();
    }

    private void RefreshDetail()
    {
        // 找到uid对应的动态数据
        PackageLocalItem localItem = GameManager.Instance.GetPackageLocalItemByUId(chooseUID);
        // 刷新详情界面
        UIDetailPanel.GetComponent<DetailsPanel>().Refresh(localItem, this);
    }

    public void RefreshScroll()
    {
        // 清理滚动容器中原本的物品
        RectTransform scrollContent = UIScrollView.GetComponent<ScrollRect>().content;

        for (int i = 0; i < scrollContent.childCount; i++)
        {
            Destroy(scrollContent.GetChild(i).gameObject);
        }

        // 获取所有物品并排序
        List<PackageLocalItem> allItems = GameManager.Instance.GetSortPackageLocalData();
       

        // 创建背包物品显示数据列表
        List<CellDisplayData> displayDataList = new List<CellDisplayData>();

        // 计算每个物品对应格子应该显示的数量
        foreach (PackageLocalItem localData in allItems)
        {
            
            PackageTableItem tableItem = GameManager.Instance.GetPackageItemById(localData.id);
            
            int remaining = localData.num;

            // 创建足够的格子来显示物品
            while (remaining > 0)
            {
                int displayNum = Mathf.Min(remaining, tableItem.maxStack);
                displayDataList.Add(new CellDisplayData
                {
                    localData = localData,
                    displayNum = displayNum
                });

                remaining -= displayNum;
            }
        }

        // 实例化背包中物品
        foreach (CellDisplayData displayData in displayDataList)
        {
            // 实例化物品格子
            Transform PackageUIItem = Instantiate(PackageUIItemPrefab.transform, scrollContent) as Transform;
            PackageCell packageCell = PackageUIItem.GetComponent<PackageCell>();
            packageCell.Refresh(displayData.localData, displayData.displayNum, this);
        }
        // 实例化剩余空白格子
        for (int i = 1; i <= remainingCell; i++)
        {
            Transform PackageUICell = Instantiate(packageUICellPrefab.transform, scrollContent) as Transform;
        }
    }
    

    public void ShowDetailPanel()
    {
        if (UIDetailPanel != null)
        {
            UIDetailPanel.gameObject.SetActive(true);
        }
        else
        {
            Debug.LogError("DetailsPanel reference is missing!");
        }
    }
   
    private void OnClickCloseBtn()
    {
        print(">>>>> PackagePanel已关");
        ClosePanel();
    }

    private void OnClickDetailExitBtn()
    {
        print(">>>>> DetailPanel已关s");
        UIDetailPanel.gameObject.SetActive(false);
    }

}
