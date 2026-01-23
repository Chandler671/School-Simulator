using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor.PackageManager.Requests;
using UnityEngine;
using UnityEngine.UI;

public class NewGuideView : BasePanel
{
    private Transform group;
    //人物展示立绘
    private Transform heroNode;
    private Transform nameTxt;
    private Transform genderTxt;
    private Transform detailTxt;

    private GameObject heroSpEff;
    //主角配置
    private hero_detail hero_detail;
    //当前选中的hero id
    private int heroId;

    protected override void onInitCom()
    {
        base.onInitCom();
        this.group = transform.Find("container/group");
        this.heroNode = transform.Find("showHero/heroNode");
        this.nameTxt = transform.Find("showHero/name/nameTxt");
        this.genderTxt = transform.Find("showHero/gender/genderTxt");
        this.detailTxt = transform.Find("showHero/detail/detailTxt");

        this.name = UIConst.NewGuideView;
    }

    protected override void onOpenPanel()
    {
        base.onOpenPanel();
        hero_detail = Resources.Load<hero_detail>("Data/hero_detail");
        for (int i = 1; i <= 4; i++)
        {
            //初始化头像
            Image headIcon = this.group.Find($"item{i}/head/headIcon").GetComponent<Image>();
            headIcon.sprite = Resources.Load<Sprite>(hero_detail.DataList[i - 1].imgPath);
            if (i == 1)
            {
                this.group.Find($"item{i}/select").gameObject.SetActive(true);
                ResetShowHero(i);
            }
            else
            {
                this.group.Find($"item{i}/select").gameObject.SetActive(false);
            }
        }
    }

    private void ResetShowHero(int id)
    {
        hero_detail_json data = hero_detail.getDataByID(id);
        heroId = id;
        //设置人物立绘
        for (int i = 1; i <= 4; i++)
        {
            if (data.id == i)
            {
                this.heroNode.GetChild(i - 1).gameObject.SetActive(true);
            }
            else
            {
                this.heroNode.GetChild(i - 1).gameObject.SetActive(false);
            }
        }
        //设置名字
        nameTxt.GetComponent<Text>().text = data.name;
        //设置性别
        genderTxt.GetComponent<Text>().text = data.gender;
        //设置简介
        detailTxt.GetComponent<Text>().text = data.description;
    }

    public void onToggleGroup()
    {
        for (int index = 1; index <= 4; index++)
        {
            Transform item = this.group.Find("item" + index);
            if (item.GetComponent<Toggle>().isOn)
            {
                item.Find("select").gameObject.SetActive(true);
                ResetShowHero(index);
            }
            else
            {
                item.Find("select").gameObject.SetActive(false);
            }
        }
    }

    public void onClickNext()
    {
        PlayerJson playerJson = PlayerModel.Instance.playerJson;
        playerJson.heroId = heroId;
        playerJson.isNewPlayer = false;
        PlayerModel.Instance.SavePlayerData();
        ClosePanel();
    }
}
