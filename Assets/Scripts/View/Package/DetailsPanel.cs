using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class DetailsPanel : BasePanel
{
    private Transform UITitile;
    private Transform UIDescription;
    private Transform UIIcon;
    private Transform UINum;
    private PackageLocalItem packageLocalData;
    private PackageTableItem packageTableItem;
    private PackagePanel uiParent;


    // private void Test()
    // {
    //     Refresh(GameManager.Instance.GetPackageLocalData()[1], null);
    // }

    protected override void onInitCom()
    {
        UITitile = transform.Find("Top/Titile");
        UIDescription = transform.Find("Center/Description");
        UIIcon = transform.Find("Center/Image");
        UINum = transform.Find("Center/Amount");
    }


    public void Refresh(PackageLocalItem packageLocalData, PackagePanel uiParent)
    {
        // 初始化：动态数据、静态数据、父物品逻辑
        this.packageLocalData = packageLocalData;
        this.packageTableItem = GameManager.Instance.GetPackageItemById(packageLocalData.id);
        this.uiParent = uiParent;
        // 数量
        UINum.GetComponent<Text>().text = string.Format("拥有：{0}", this.packageLocalData.num.ToString());
        // 简短描述
        UIDescription.GetComponent<Text>().text = this.packageTableItem.description;
        // 物品名称
        UITitile.GetComponent<Text>().text = this.packageTableItem.name;
        // 图片加载
        Texture2D t = (Texture2D)Resources.Load(this.packageTableItem.imgPath);
        Sprite temp = Sprite.Create(t, new Rect(0, 0, t.width, t.height), new Vector2(0, 0));
        UIIcon.GetComponent<Image>().sprite = temp;
    }
}
