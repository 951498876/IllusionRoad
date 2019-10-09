using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class ShopPanel : MonoBehaviour
{
    private ManagerVars vars;

    private Transform parent;
    private Text txt_name;
    private Text txt_dmdCnt;
    private Button btn_back;
    private Button btn_select;
    private Button btn_buy;
    private int currentIndex;//选中皮肤的序号


    private void Awake()
    {
        vars = ManagerVars.GetManagerVars();
        EventCenter.AddListener(EventDefine.ShowShopPanel, Show);
    }

    private void Start()
    {
        Init();
    }
    private void OnDestroy()
    {
        EventCenter.RemoveListener(EventDefine.ShowShopPanel, Show);
    }

    /// <summary>
    /// 返回按钮点击
    /// </summary>
    private void OnBackBtnClick()
    {
        EventCenter.Broadcast(EventDefine.PlayClickBtn);
        EventCenter.Broadcast(EventDefine.ShowMainPanel);
        gameObject.SetActive(false);
    }

    /// <summary>
    /// 购买按钮点击
    /// </summary>
    private void OnBuyBtnClick()
    {
        EventCenter.Broadcast(EventDefine.PlayClickBtn);
        int price = int.Parse(btn_buy.GetComponentInChildren<Text>().text);
        if (price > GameManager.Instance.GetTotalDmdCnt())
        {
            EventCenter.Broadcast(EventDefine.ShowHint, "钻石不足，不能购买");
            return;
        }
        else
        {
            GameManager.Instance.UpdateTotalDmdCnt(-price);
            GameManager.Instance.SetSkinUnlocked(currentIndex);
            parent.GetChild(currentIndex).GetChild(0).GetComponentInChildren<Image>().color = Color.white;
        }
    }

    /// <summary>
    /// 选择按钮点击
    /// </summary>
    private void OnSelectBtnClick()
    {
        EventCenter.Broadcast(EventDefine.PlayClickBtn);
        EventCenter.Broadcast(EventDefine.ChangeSkin, currentIndex);
        GameManager.Instance.SetSelectedSkin(currentIndex);
        EventCenter.Broadcast(EventDefine.ShowMainPanel);
        gameObject.SetActive(false);
    }


    private void Show()
    {
        gameObject.SetActive(true);
    }

    private void Init()
    {
        parent = transform.Find("ScrollRect/parent");
        txt_name = transform.Find("txt_name").GetComponent<Text>();
        txt_dmdCnt = transform.Find("Diamond/txt_dmdCnt").GetComponent<Text>();
        btn_back = transform.Find("btn_back").GetComponent<Button>();
        btn_back.onClick.AddListener(OnBackBtnClick);
        btn_buy = transform.Find("btn_buy").GetComponent<Button>();
        btn_buy.onClick.AddListener(OnBuyBtnClick);
        btn_select = transform.Find("btn_select").GetComponent<Button>();
        btn_select.onClick.AddListener(OnSelectBtnClick);


        gameObject.SetActive(false);

        parent.GetComponent<RectTransform>().sizeDelta = new Vector2((vars.skins.Count + 2) * 160, 300);

        //排列生成的皮肤选项
        for (int i = 0; i < vars.skins.Count; i++)
        {
            GameObject go = Instantiate(vars.skinChoserPrefab, parent);

            if (!GameManager.Instance.GetIfSkinUnlocked(i))//判断皮肤有无解锁
            {
                go.GetComponentInChildren<Image>().color = Color.grey;
            }
            else
            {
                go.GetComponentInChildren<Image>().color = Color.white;
            }

            go.GetComponentInChildren<Image>().sprite = vars.skins[i];
            go.transform.localPosition = new Vector3((i + 1) * 160, 0, 0);
        }
        //按照当前选择皮肤设置滚动条的位置,打开页面直接定位到选中的皮肤
        parent.transform.localPosition = new Vector3(GameManager.Instance.GetSelectedSkin() * -160, 0);
    }

    private void Update()
    {
        currentIndex = (int)Mathf.Round(parent.transform.localPosition.x / -160f);
        currentIndex = (currentIndex < 0) ? 0 : currentIndex;
        currentIndex = (currentIndex > parent.childCount-1) ? parent.childCount-1 : currentIndex;
        //Debug.Log(currentIndex);
        if (Input.GetMouseButtonUp(0))
        {
            parent.transform.DOLocalMoveX(currentIndex * -160, 0.2f);
            //parent.transform.localPosition = new Vector3(currentIndex * -160, 0, 0);
        }
        SetSkinSize(currentIndex);
        SetSkinName(currentIndex);
    }

    private void SetSkinSize(int index)
    {
        for (int i = 0; i < parent.childCount; i++)
        {
            parent.GetChild(i).GetChild(0).GetComponent<RectTransform>().sizeDelta = (index == i) ? new Vector2(160, 160) : new Vector2(80, 80);
        }
    }

    private void SetSkinName(int selectIndex)
    {
        txt_name.text = vars.skinNames[selectIndex];
        txt_dmdCnt.text = GameManager.Instance.GetTotalDmdCnt().ToString();

        //未解锁
        if (!GameManager.Instance.GetIfSkinUnlocked(selectIndex))
        {
            btn_select.gameObject.SetActive(false);
            btn_buy.gameObject.SetActive(true);
            btn_buy.GetComponentInChildren<Text>().text = vars.skinPrices[selectIndex].ToString();
        }
        else
        {
            btn_select.gameObject.SetActive(true);
            btn_buy.gameObject.SetActive(false);
        }
    }
}
