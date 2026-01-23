using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class PackageCell : MonoBehaviour, IPointerClickHandler, IPointerEnterHandler, IPointerExitHandler
{
    private Transform UIIcon;
    private Transform UIItemNum;
    private Transform UIItemName;
    private Transform UIDetailPanel;
    private GameObject current;
    private PackageLocalItem packageLocalData;
    private PackageTableItem packageTableItem;
    private PackagePanel uiParent;


    private void Awake()
    {
        InitUIName();
        InitClick();
    }
    private void InitUIName()
    {
        UIIcon = transform.Find("Top/Icon");
        UIItemNum = transform.Find("Top/Num");
        UIItemName = transform.Find("Bottom/Name");
        current = gameObject;
    }

    private void InitClick()
    {
        current.GetComponent<Button>().onClick.AddListener(OnClickPackageUIItem);
    }
    
     public void Refresh(PackageLocalItem packageLocalData, int displayNum, PackagePanel uiParent)
    {
        // 数据初始化
        this.packageLocalData = packageLocalData;
        this.packageTableItem = GameManager.Instance.GetPackageItemById(packageLocalData.id);
        this.uiParent = uiParent;
        
        // 直接使用传入的显示数量
        UIItemNum.GetComponent<Text>().text = displayNum.ToString();
        
        // 物品的图片
        Texture2D t = (Texture2D)Resources.Load(this.packageTableItem.imgPath);
        if (t != null)
        {
            Sprite temp = Sprite.Create(t, new Rect(0, 0, t.width, t.height), new Vector2(0, 0));
            UIIcon.GetComponent<Image>().sprite = temp;
        }
        else
        {
            Debug.LogError($"加载图片失败: {this.packageTableItem.imgPath}");
        }
        
        // 物品的名字
        UIItemName.GetComponent<Text>().text = this.packageTableItem.name;
    }


    private void OnClickPackageUIItem()
    {
        // 直接查找激活的PackagePanel
        PackagePanel panel = FindObjectOfType<PackagePanel>();
        if (panel != null)
        {
            // 使用面板的公共方法，而不是直接访问变量
            panel.ShowDetailPanel();
        }
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (this.uiParent.chooseUID == this.packageLocalData.uid)
            return;
        // 根据点击设置最新的uid -> 进而刷新详情界面
        this.uiParent.chooseUID = this.packageLocalData.uid;
        // 直接查找激活的PackagePanel
        PackagePanel panel = FindObjectOfType<PackagePanel>();
        if (panel != null)
        {
            // 使用面板的公共方法，而不是直接访问变量
            panel.ShowDetailPanel();
        }
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        // Debug.Log("OnPointerEnter: " + eventData.ToString());
        // UIMouseOverAni.gameObject.SetActive(true);
        // UIMouseOverAni.GetComponent<Animator>().SetTrigger("In");
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        // Debug.Log("OnPointerExit: " + eventData.ToString());
        // UIMouseOverAni.GetComponent<Animator>().SetTrigger("Out");
    }
}
