using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
/// <summary>
/// 控制UI的脚本
/// </summary>
public class MainPanel : MonoBehaviour
{
    private Button btn_Start;
    private Button btn_Shop;
    private Button btn_Reset;
    private Button btn_Rank;
    private Button btn_Sound;

    private ManagerVars vars;

    private void Awake()
    {
        vars = ManagerVars.GetManagerVars();
        EventCenter.AddListener(EventDefine.ShowMainPanel, Show);
        EventCenter.AddListener<int>(EventDefine.ChangeSkin, ChangeSkin);
        Init();
    }
    private void OnDestroy()
    {
        EventCenter.RemoveListener(EventDefine.ShowMainPanel, Show);
        EventCenter.RemoveListener<int>(EventDefine.ChangeSkin, ChangeSkin);
    }

    private void Start()
    {
        //Awake时不能确保已经登录事件，于是放在Start中执行
        if (GameData.ifRetryGame)
        {
            EventCenter.Broadcast(EventDefine.ShowGamePanel);
            gameObject.SetActive(false);
        }
        Sound();
        ChangeSkin(GameManager.Instance.GetSelectedSkin());
    }

    private void ChangeSkin(int index)
    {

        btn_Shop.transform.GetChild(0).GetComponent<Image>().sprite = vars.skins[index];
    }

    private void Show()
    {
        gameObject.SetActive(true);
    }

    private void Init()
    {
        btn_Start = transform.Find("btn_start").GetComponent<Button>();
        btn_Shop = transform.Find("Btns/btn_shop").GetComponent<Button>();
        btn_Reset = transform.Find("Btns/btn_reset").GetComponent<Button>();
        btn_Rank = transform.Find("Btns/btn_rank").GetComponent<Button>();
        btn_Sound = transform.Find("Btns/btn_sound").GetComponent<Button>();

        btn_Start.onClick.AddListener(OnStartButClick);
        btn_Shop.onClick.AddListener(OnShopButClick);
        btn_Reset.onClick.AddListener(OnResetButClick); 
        btn_Rank.onClick.AddListener(OnRankButClick);
        btn_Sound.onClick.AddListener(OnSoundButClick);
    }
    /// <summary>
    /// 开始按钮点击
    /// </summary>
    private void OnStartButClick()
    {
        EventCenter.Broadcast(EventDefine.PlayClickBtn);
        GameManager.Instance.IsGameStarted = true;
        EventCenter.Broadcast(EventDefine.ShowGamePanel);
        gameObject.SetActive(false);
    }
    /// <summary>
    /// 商店按钮点击
    /// </summary>
    private void OnShopButClick()
    {
        EventCenter.Broadcast(EventDefine.PlayClickBtn);
        EventCenter.Broadcast(EventDefine.ShowShopPanel);
    }

    /// <summary>
    /// 重置按钮点击
    /// </summary>
    private void OnResetButClick()
    {
        EventCenter.Broadcast(EventDefine.PlayClickBtn);
        EventCenter.Broadcast(EventDefine.ShowResetPanel);
    }




    /// <summary>
    /// 排行榜按钮点击
    /// </summary>
    private void OnRankButClick()
    {
        EventCenter.Broadcast(EventDefine.PlayClickBtn);
        EventCenter.Broadcast(EventDefine.ShowRankPanel);
    }
    /// <summary>
    /// 声音按钮点击
    /// </summary>
    private void OnSoundButClick()
    {
        EventCenter.Broadcast(EventDefine.PlayClickBtn);
        GameManager.Instance.SwitchIsMusicOn();

        Sound();
    }

    private void Sound()
    {
        if (GameManager.Instance.GetIsMusicOn())
        {
            btn_Sound.transform.GetChild(0).GetComponent<Image>().sprite = vars.musicOn;
        }
        else
        {
            btn_Sound.transform.GetChild(0).GetComponent<Image>().sprite = vars.musicOff;
        }
        EventCenter.Broadcast(EventDefine.IsMusicOn, GameManager.Instance.GetIsMusicOn());
    }
}
