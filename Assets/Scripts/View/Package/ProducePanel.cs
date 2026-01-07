using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ProducePanel : BasePanel
{

    private Transform UIIcon;
    private Transform UIName;
    private Transform UIOwnNumber;
    private Transform UIDescription;
    private Transform UIMaterialFirstIcon;
    private Transform UIMaterialFirstNeed;
    private Transform UIMaterialSecondIcon;
    private Transform UIMaterialSecondNeed;
    private Transform UIMaterialThirdIcon;
    private Transform UIMaterialThirdNeed;
    private Transform UIScrollView;
    private Transform UIAddOneBtn;
    private Transform UIDeleteOneBtn;
    private Transform UIProduceBtn;
    private Transform UIProduceNum;
    private Transform UIClosePanelBtn;
    private Transform UIRight;
    public int initProduceNum;
    private PackageLocalItem packageLocalData;
    private PackageTableItem packageTableItem;
    public GameObject produceRecipeItemPrefab;
    private int currentSelectedId = -1; // 当前选中的物品ID
    private GameObject currentSelectedRecipeItem; // 当前选中的配方物品
    private List<int> recipeIds = new List<int>() { 1, 2, 3 }; // 配方物品ID列表
    private Dictionary<int, int> materialRequirements = new Dictionary<int, int>(); // 材料需求字典

     

    void Start()
    {
        InitUI();
        UIRight.gameObject.SetActive(false);
        RefreshScroll();
    }
    private void InitUI()
    {
        InitUIName();
        InitUIClick();
    }

    // private void RefreshUI()
    // {
    //     RefreshScroll();
    // }

    private void RefreshScroll()
    {
        // 添加空值检查
        if (UIScrollView == null)
        {
            Debug.LogError("UIScrollView is null! Check InitUIName()");
            return;
        }
        ScrollRect scrollRect = UIScrollView.GetComponent<ScrollRect>();

        if (scrollRect == null || scrollRect.content == null)
        {
            Debug.LogError("ScrollRect or content is null! Make sure ScrollView has ScrollRect component");
            return;
        }

        RectTransform scrollContent = scrollRect.content;

        // 清理滚动容器中原本的物品
        for (int i = 0; i < scrollContent.childCount; i++)
        {
            Destroy(scrollContent.GetChild(i).gameObject);
        }

        currentSelectedRecipeItem = null; // 重置当前选中

        // 生成配方物品
        foreach (int id in recipeIds)
        {
            PackageTableItem tableItem = GameManager.Instance.GetPackageItemById(id);
            if (tableItem != null)
            {
                Transform item = Instantiate(produceRecipeItemPrefab.transform, scrollContent);
                item.name = "Recipe_" + id;

                // 获取组件引用
                Transform icon = item.Find("Top/Icon");
                Transform nameText = item.Find("Bottom/Name");
                Transform numText = item.Find("Top/Num");

                // 隐藏数量显示
                if (numText != null) numText.gameObject.SetActive(false);

                // 设置图标和名称
                Texture2D t = (Texture2D)Resources.Load(tableItem.imgPath);
                if (t != null)
                {
                    Sprite temp = Sprite.Create(t, new Rect(0, 0, t.width, t.height), new Vector2(0, 0));
                    icon.GetComponent<Image>().sprite = temp;
                }
                nameText.GetComponent<Text>().text = tableItem.name;

                // 添加选中状态引用
                Transform selectedIndicator = item.Find("SelectedIndicator"); // 根据实际预制体结构调整
                if (selectedIndicator != null)
                {
                    selectedIndicator.gameObject.SetActive(false);
                }
                // 添加点击事件
                Button btn = item.GetComponent<Button>();
                if (btn == null) btn = item.gameObject.AddComponent<Button>();
                btn.onClick.RemoveAllListeners();
                btn.onClick.AddListener(() => OnRecipeSelected(id, item.gameObject));
            }
        }
    }

    // 配方选择处理
    private void OnRecipeSelected(int id, GameObject recipeItem)
    {
        // 取消之前选中的状态
        if (currentSelectedRecipeItem != null)
        {
            Transform prevSelected = currentSelectedRecipeItem.transform.Find("SelectedIndicator");
            if (prevSelected != null) prevSelected.gameObject.SetActive(false);
        }

        // 设置新的选中状态
        currentSelectedRecipeItem = recipeItem;
        Transform currentSelected = recipeItem.transform.Find("SelectedIndicator");
        if (currentSelected != null) currentSelected.gameObject.SetActive(true);

        currentSelectedId = id;
        packageTableItem = GameManager.Instance.GetPackageItemById(id);

        // 刷新右侧详情
        RefreshDetail(packageTableItem);

        // 重置制作数量
        initProduceNum = 1;
        UIProduceNum.GetComponent<Text>().text = $"制作×{initProduceNum}".ToString();
        UIRight.gameObject.SetActive(true);
    }

    // 刷新右侧详情
    private void RefreshDetail(PackageTableItem packageTableItem)
    {
        List<PackageLocalItem> localItems = GameManager.Instance.GetPackageLocalData();
        if (packageTableItem == null) return;

        // 设置物品详情
        Texture2D t = (Texture2D)Resources.Load(packageTableItem.imgPath);
        if (t != null)
        {
            Sprite temp = Sprite.Create(t, new Rect(0, 0, t.width, t.height), new Vector2(0, 0));
            UIIcon.GetComponent<Image>().sprite = temp;
        }
        UIName.GetComponent<Text>().text = packageTableItem.name;
        foreach (PackageLocalItem localItem in localItems)
        {
            if (localItem.id == packageTableItem.id)
            {
                UIOwnNumber.GetComponent<Text>().text = $"拥有：{localItem.num}";
            }
        }
        UIDescription.GetComponent<Text>().text = packageTableItem.description;

        // 解析材料需求
        materialRequirements.Clear();
        ParseMaterialRequirements(packageTableItem.MaterialRequirement);

        // 设置材料显示
        SetMaterialDisplay(UIMaterialFirstIcon, UIMaterialFirstNeed, 0);
        SetMaterialDisplay(UIMaterialSecondIcon, UIMaterialSecondNeed, 1);
        SetMaterialDisplay(UIMaterialThirdIcon, UIMaterialThirdNeed, 2);
    }

    // 解析材料需求
    private void ParseMaterialRequirements(string requirements)
    {
        if (string.IsNullOrEmpty(requirements)) return;

        string[] pairs = requirements.Split(',');
        foreach (string pair in pairs)
        {
            string[] parts = pair.Split(':');
            if (parts.Length == 2 && int.TryParse(parts[0], out int id) && int.TryParse(parts[1], out int amount))
            {
                materialRequirements[id] = amount;
            }
        }
    }

    // 设置材料显示
    private void SetMaterialDisplay(Transform iconTransform, Transform textTransform, int index)
    {
        if (index >= materialRequirements.Count)
        {
            iconTransform.gameObject.SetActive(false);
            textTransform.gameObject.SetActive(false);
            return;
        }

        int i = 0;
        foreach (var kvp in materialRequirements)
        {
            if (i == index)
            {
                // 获取材料数据
                PackageTableItem materialItem = GameManager.Instance.GetPackageItemById(kvp.Key);
                if (materialItem == null)
                {
                    Debug.LogError($"Material item not found for ID: {kvp.Key}");
                    break;
                }
                List<PackageLocalItem> localItems = GameManager.Instance.GetPackageLocalData();
                int ownedAmount = 0;
                foreach (var item in localItems)
                {
                    if (item.id == kvp.Key) ownedAmount += item.num;
                }

                // 设置图标 - 添加空值检查
                if (iconTransform != null)
                {
                    Texture2D t = (Texture2D)Resources.Load(materialItem.imgPath);
                    if (t != null)
                    {
                        Sprite temp = Sprite.Create(t, new Rect(0, 0, t.width, t.height), new Vector2(0, 0));
                        Image iconImage = iconTransform.GetComponent<Image>();
                        if (iconImage != null)
                        {
                            iconImage.sprite = temp;
                        }
                        else
                        {
                        Debug.LogError("IconTransform does not have an Image component");
                        }
                    }
                    else
                    {
                        Debug.LogError($"Failed to load texture at path: {materialItem.imgPath}");
                    }
                    iconTransform.gameObject.SetActive(true);
                }
                else
                {
                    Debug.LogError("iconTransform is null");
                }
                // 设置文本 - 添加空值检查
                if (textTransform != null)
                {
                    Text textComponent = textTransform.GetComponent<Text>();
                    if (textComponent != null)
                    {
                        textComponent.text = $"({ownedAmount} / {kvp.Value}) {materialItem.name}".ToString();
                        textComponent.color = ownedAmount >= kvp.Value ? new Color(91f/255f, 76f/255f, 122f/255f) : Color.red;
                        textTransform.gameObject.SetActive(true);
                    }
                    else
                    {
                        Debug.LogError("textTransform does not have a Text component");
                    }
                }
                else
                {
                    Debug.LogError("textTransform is null");
                }
                break;
            }
            i++;
        }
    }



    private void InitUIName()
    {
        UIIcon = transform.Find("Right/ProduceMenu/Top/Icon");
        UIName = transform.Find("Right/ProduceMenu/Top/Name");
        UIOwnNumber = transform.Find("Right/ProduceMenu/Top/OwnNumber");
        UIDescription = transform.Find("Right/ProduceMenu/Top/Description");
        UIMaterialFirstIcon = transform.Find("Right/ProduceMenu/Bottom/Material/Top/Bg/Icon");
        UIMaterialFirstNeed = transform.Find("Right/ProduceMenu/Bottom/Material/Top/NumberAndName");
        UIMaterialSecondIcon = transform.Find("Right/ProduceMenu/Bottom/Material/Center/Bg/Icon");
        UIMaterialSecondNeed = transform.Find("Right/ProduceMenu/Bottom/Material/Center/NumberAndName");
        UIMaterialThirdIcon = transform.Find("Right/ProduceMenu/Bottom/Material/Bottom/Bg/Icon");
        UIMaterialThirdNeed = transform.Find("Right/ProduceMenu/Bottom/Material/Bottom/NumberAndName");
        UIScrollView = transform.Find("Left/Scroll View");
        UIAddOneBtn = transform.Find("Right/ProduceMenu/Bottom/ProduceButton/Right/Image");
        UIDeleteOneBtn = transform.Find("Right/ProduceMenu/Bottom/ProduceButton/Left/Image");
        UIProduceBtn = transform.Find("Right/ProduceMenu/Bottom/ProduceButton/Center/Image");
        UIClosePanelBtn = transform.Find("Right/ShutDown/Button");
        UIProduceNum = transform.Find("Right/ProduceMenu/Bottom/ProduceButton/Center/Image/Text");
        UIRight = transform.Find("Right");
    }

    private void InitUIClick()
    {
        UIClosePanelBtn.GetComponent<Button>().onClick.AddListener(OnClickClosePanelBtn);
        UIAddOneBtn.GetComponent<Button>().onClick.AddListener(OnClickAddOneBtn);
        UIDeleteOneBtn.GetComponent<Button>().onClick.AddListener(OnClickDeleteOneBtn);
        UIProduceBtn.GetComponent<Button>().onClick.AddListener(OnClickProduceBtn);
    }

    private void OnClickClosePanelBtn()
    {
        UIManager.Instance.ClosePanel(UIConst.ProducePanel);
    }

    private void OnClickAddOneBtn()
    {
        initProduceNum++;
        UIProduceNum.GetComponent<Text>().text = $"制作×{initProduceNum}".ToString();
    }

    private void OnClickDeleteOneBtn()
    {
        if (initProduceNum > 1)
        {
            initProduceNum--;
            UIProduceNum.GetComponent<Text>().text = $"制作×{initProduceNum}".ToString();
        }
    }

    private void OnClickProduceBtn()
    {
        if (currentSelectedId == -1) return;

        // 检查材料是否足够
        if (!CheckMaterialsAvailable()) return;

        // 扣除材料
        ConsumeMaterials();

        // 添加制作物品
        AddProducedItem();

        // 保存数据
        PackageLocalData.Instance.SavePackage();

        // 刷新PackagePanel
        RefreshPackagePanel();
    }

    // 检查材料是否足够
    private bool CheckMaterialsAvailable()
    {
        List<PackageLocalItem> localItems = GameManager.Instance.GetPackageLocalData();

        foreach (var kvp in materialRequirements)
        {
            packageTableItem = GameManager.Instance.GetPackageItemById(kvp.Key);
            int required = kvp.Value * initProduceNum;
            int owned = 0;

            foreach (var item in localItems)
            {
                if (item.id == kvp.Key) owned += item.num;
            }

            if (owned < required)
            {
                Debug.Log($"{packageTableItem.name}材料不足! , 需要 {required}, 当前只有 {owned}");
                return false;
            }
        }
        return true;
    }

    // 扣除材料
    private void ConsumeMaterials()
    {
        List<PackageLocalItem> localItems = PackageLocalData.Instance.items;

        foreach (var kvp in materialRequirements)
        {
            int amountToConsume = kvp.Value * initProduceNum;

            for (int i = localItems.Count - 1; i >= 0; i--)
            {
                if (localItems[i].id == kvp.Key)
                {
                    if (localItems[i].num >= amountToConsume)
                    {
                        localItems[i].num -= amountToConsume;
                        amountToConsume = 0;
                    }
                    else
                    {
                        amountToConsume -= localItems[i].num;
                        localItems[i].num = 0;
                    }

                    // 移除数量为0的物品
                    if (localItems[i].num == 0)
                    {
                        localItems.RemoveAt(i);
                    }

                    if (amountToConsume <= 0) break;
                }
            }
        }
    }

    // 添加制作物品
    private void AddProducedItem()
    {
        List<PackageLocalItem> localItems = PackageLocalData.Instance.items;

        // 查找是否已有该物品
        foreach (var item in localItems)
        {
            if (item.id == currentSelectedId)
            {
                item.num += initProduceNum;
                break;
            }
        }
    }

   // 刷新PackagePanel
    private void RefreshPackagePanel()
    {
        PackagePanel packagePanel = UIManager.Instance.GetPanel(UIConst.PackagePanel) as PackagePanel;
        if (packagePanel != null && packagePanel.isActiveAndEnabled)
        {
            packagePanel.RefreshScroll();
        }
    }
}
