using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

 /// <summary>
/// 主要的全局公共方法
/// </summary>
public class G
{

    /// <summary>
    /// 显示提示信息
    /// </summary>
    /// <param name="message">要显示的文本</param>
    public static void ShowMessage(string message)
    {
        if (message == "")
        {
            Debug.LogError("提示飘字为空字符串！！");
            return;
        }

        Transform uiRoot = GameObject.Find("Canvas").transform;
        GameObject showMessage = Resources.Load<GameObject>("Prefab/common/ShowMessage");
        if (!uiRoot)
        {
            uiRoot = new GameObject("Canvas").transform;
        }
        GameObject showMessage1 = GameObject.Instantiate(showMessage, uiRoot, false);
        ShowMessage showMessage2 = showMessage1.gameObject.GetComponent<ShowMessage>();
        showMessage2.ContentTxt = message;
    }
}
