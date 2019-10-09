using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
/// <summary>
/// 提示界面
/// </summary>
public class Hint : MonoBehaviour
{
    private Image img_bg;
    private Text txt_hint;
    private void Awake()
    {
        img_bg = GetComponent<Image>();
        txt_hint = GetComponentInChildren<Text>();
        img_bg.color = new Color(img_bg.color.r, img_bg.color.g, img_bg.color.b,0);
        txt_hint.color = new Color(txt_hint.color.r, txt_hint.color.g, txt_hint.color.b, 0);

        EventCenter.AddListener<string>(EventDefine.ShowHint, Show);
    }
    private void OnDestroy()
    {
        EventCenter.RemoveListener<string>(EventDefine.ShowHint, Show);
    }

    private void Show(string text)
    {
        StopCoroutine("Delay");
        transform.localPosition = new Vector3(0, -70, 0);
        transform.DOLocalMoveY(0, 0.3f).OnComplete(()=>
        {
            StartCoroutine("Delay");
        });
        img_bg.DOColor(new Color(img_bg.color.r, img_bg.color.g, img_bg.color.b, 0.4f), 0.1f);
        txt_hint.DOColor(new Color(txt_hint.color.r, txt_hint.color.g, txt_hint.color.b, 0.4f), 0.1f);
        txt_hint.text = text;
    }

    private IEnumerator Delay()
    {
        yield return new WaitForSeconds(1f);
        transform.DOLocalMoveY(70, 0.3f);
        img_bg.DOColor(new Color(img_bg.color.r, img_bg.color.g, img_bg.color.b, 0), 0.1f);
        txt_hint.DOColor(new Color(txt_hint.color.r, txt_hint.color.g, txt_hint.color.b, 0), 0.1f);
    }
}
