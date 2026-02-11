using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainCityLayer : BasePanel
{
    protected override void onInitCom()
    {
        base.onInitCom();
        EventManager.AddListener<login_Success>(onSuccess);
    }

    private void onSuccess(login_Success e)
    {
        G.ShowMessage($"登录成功！欢迎用户：{e.UserName}");
    }
}
