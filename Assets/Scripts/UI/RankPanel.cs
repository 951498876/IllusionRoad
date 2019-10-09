using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class RankPanel : MonoBehaviour
{
    private Button btn_back;
    private GameObject scoreList;
    public Text[] scores;

    private void Awake()
    {
        EventCenter.AddListener(EventDefine.ShowRankPanel, Show);

        btn_back = transform.Find("btn_back").GetComponent<Button>();
        scoreList = transform.Find("scoreList").gameObject;

        btn_back.onClick.AddListener(OnBackBtnClick);

        btn_back.GetComponent<Image>().color = new Color(btn_back.GetComponent<Image>().color.r,
            btn_back.GetComponent<Image>().color.g, btn_back.GetComponent<Image>().color.b, 0);
        scoreList.transform.localScale = Vector3.zero;
        gameObject.SetActive(false);
    }

    private void OnDestroy()
    {
        EventCenter.RemoveListener(EventDefine.ShowRankPanel, Show);
    }

    private void Show()
    {
        gameObject.SetActive(true);
        btn_back.GetComponent<Image>().DOColor(new Color(btn_back.GetComponent<Image>().color.r,
            btn_back.GetComponent<Image>().color.g, btn_back.GetComponent<Image>().color.b, 0.3f), 0.3f);
        scoreList.transform.DOScale(Vector3.one, 0.3f);

        int[] arr = GameManager.Instance.GetScoreArr();
        for (int i = 0; i < arr.Length; i++)
        {
            scores[i].text = arr[i].ToString();
        }
    }

    private void OnBackBtnClick()
    {
        btn_back.GetComponent<Image>().DOColor(new Color(btn_back.GetComponent<Image>().color.r,
            btn_back.GetComponent<Image>().color.g, btn_back.GetComponent<Image>().color.b, 0), 0.3f);
        scoreList.transform.DOScale(Vector3.zero, 0.3f).OnComplete(()=> 
        {
            gameObject.SetActive(false);
        });
    }
}
