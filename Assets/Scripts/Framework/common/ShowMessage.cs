using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class ShowMessage : MonoBehaviour
{
    //显示文本组件
    public Text showMessage;
    // 要显示的文本内容
    private string contentTxt;
    public string ContentTxt
    {
        set
        {
            this.contentTxt = value;
        }
    }
    // 起始位置
    private Vector3 startPos;
    //终止位置
    private Vector3 endPos;

    void Awake()
    {
        startPos = new Vector3(0, 563, 0);
        endPos = new Vector3(0, 410, 0);

    }

    void Start()
    {
        this.transform.localPosition = startPos;

        if (showMessage != null)
        {
            showMessage.text = contentTxt;
            this.gameObject.SetActive(true);
            DOTween.Sequence().Append(this.transform.DOLocalMove(endPos, 0.5f))
            .AppendInterval(1f)
            .AppendCallback(() =>
            {
                Destroy(this.gameObject);
            });
        }
    }

    void OnDestroy()
    {
        transform.DOKill();
    }
}
