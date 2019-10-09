using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GamePanel : MonoBehaviour
{
    private Button btn_pause;
    private Button btn_restart;
    private Text txt_score;
    private Text txt_dmdCnt;

    private void Awake()
    {
        EventCenter.AddListener(EventDefine.ShowGamePanel, Show);
        EventCenter.AddListener<int>(EventDefine.UpdateScoreOfPanel, UpdateScore);
        EventCenter.AddListener<int>(EventDefine.UpdateDmdOfPanel, UpdateDmdCnt);

        Init();
    }
    private void OnDestroy()
    {
        EventCenter.RemoveListener(EventDefine.ShowGamePanel, Show);
        EventCenter.RemoveListener<int>(EventDefine.UpdateScoreOfPanel, UpdateScore);
        EventCenter.RemoveListener<int>(EventDefine.UpdateDmdOfPanel, UpdateDmdCnt);
    }

    private void Init()
    {
        btn_pause = transform.Find("btn_pause").GetComponent<Button>();
        btn_restart = transform.Find("btn_restart").GetComponent<Button>();
        txt_score = transform.Find("txt_score").GetComponent<Text>();
        txt_dmdCnt = transform.Find("Diamond/txt_dmdCnt").GetComponent<Text>();

        gameObject.SetActive(false);
        btn_restart.gameObject.SetActive(false);

        btn_pause.onClick.AddListener(OnPauseBtnClick);
        btn_restart.onClick.AddListener(OnRestartBtnClick);
    }

    /// <summary>
    /// 显示游戏界面
    /// </summary>
    private void Show()
    {
        gameObject.SetActive(true);
    }
    /// <summary>
    /// 更新游戏得分显示
    /// </summary>
    private void UpdateScore(int score)
    {
        txt_score.text = score.ToString();
    }

    /// <summary>
    /// 更新钻石得分显示
    /// </summary>
    private void UpdateDmdCnt(int dmdCnt)
    {
        txt_dmdCnt.text = dmdCnt.ToString();
    }

    /// <summary>
    /// 暂停按钮点击,游戏暂停
    /// </summary>
    private void OnPauseBtnClick()
    {
        EventCenter.Broadcast(EventDefine.PlayClickBtn);
        btn_pause.gameObject.SetActive(false);
        btn_restart.gameObject.SetActive(true);
        GameManager.Instance.IsGamePaused = true;
        Time.timeScale = 0;
    }
    /// <summary>
    /// 开始按钮点击,游戏继续
    /// </summary>
    private void OnRestartBtnClick()
    {
        EventCenter.Broadcast(EventDefine.PlayClickBtn);
        btn_restart.gameObject.SetActive(false);
        btn_pause.gameObject.SetActive(true);
        GameManager.Instance.IsGamePaused = false;

        Time.timeScale = 1;
    }
}
