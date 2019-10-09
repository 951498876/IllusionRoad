using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using DG.Tweening;

public class ResetPanel : MonoBehaviour
{
    private Button btn_yes;
    private Button btn_no;
    private Image img_bg;
    private GameObject dialog;

    private void Awake()
    {
        EventCenter.AddListener(EventDefine.ShowResetPanel, Show);
        Init();
    }
    private void OnDestroy()
    {
        EventCenter.RemoveListener(EventDefine.ShowResetPanel, Show);
    }
    private void Init()
    {

        btn_yes = transform.Find("Dialog/btn_yes").GetComponent<Button>();
        btn_no = transform.Find("Dialog/btn_no").GetComponent<Button>();
        img_bg = transform.Find("bg").GetComponent<Image>();
        dialog = transform.Find("Dialog").gameObject;

        btn_yes.onClick.AddListener(OnYesBtnClick);
        btn_no.onClick.AddListener(OnNoBtnClick);

        img_bg.color = new Color(img_bg.color.r, img_bg.color.g, img_bg.color.b, 0);
        dialog.transform.localScale = Vector3.zero;
        gameObject.SetActive(false);
    }

    private void Show()
    {
        gameObject.SetActive(true);
        img_bg.DOColor(new Color(img_bg.color.r, img_bg.color.g, img_bg.color.b, 0.3f), 0.3f);
        dialog.transform.DOScale(Vector3.one, 0.3f);
    }

    private void OnYesBtnClick()
    {
        EventCenter.Broadcast(EventDefine.PlayClickBtn);
        GameManager.Instance.ResetData();
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    private void OnNoBtnClick()
    {
        EventCenter.Broadcast(EventDefine.PlayClickBtn);
        img_bg.DOColor(new Color(img_bg.color.r, img_bg.color.g, img_bg.color.b, 0), 0.3f);
        dialog.transform.DOScale(Vector3.zero, 0.3f).OnComplete(()=>
        {
            gameObject.SetActive(false);
        });
    }
}
