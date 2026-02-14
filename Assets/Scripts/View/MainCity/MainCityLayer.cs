using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class MainCityLayer : BasePanel
{
    public List<Button> mapTiles;
    private Transform groupMap;
    private GameObject heroEff;
    private hero_detail hero_Detail;
    //人物第一次进场的位置
    private Transform heroNode;
    //场景中的人物
    public GameObject heroPref;

    protected override void onInitCom()
    {
        base.onInitCom();
        hero_Detail = Resources.Load<hero_detail>("Data/hero_detail");
        EventManager.AddListener<login_Success>(onSuccess);
    }

    protected override void onOpenPanel()
    {
        base.onOpenPanel();
        heroNode = transform.Find("XGMap/HeroNode");
        groupMap = transform.Find("XGMap/GridNode");
        for (int i = 0; i <= groupMap.childCount - 1; i++)
        {
            Button mapTile = groupMap.GetChild(i).gameObject.GetComponent<Button>();
            mapTiles.Add(mapTile);
        }
    }

    private void onSuccess(login_Success e)
    {
        G.ShowMessage($"登录成功！欢迎用户：{e.UserName}");
        //加载人物模型、实例化
        heroEff = Resources.Load<GameObject>(hero_Detail.getDataByID(e.HeroId).heroEff);
        heroPref = Instantiate(heroEff, heroNode, false);
        heroPref.transform.localScale = new Vector3(23, 23, 23);
    }

    protected override void OnDestroy()
    {
        base.OnDestroy();
        EventManager.RemoveListener<login_Success>(onSuccess);
    }
}
