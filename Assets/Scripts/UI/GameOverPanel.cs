using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GameOverPanel : MonoBehaviour
{
    public Text txt_score, txt_score_max, txt_addDmdCount;
    public Button btn_retry, btn_rank, btn_home;
    public Image img_new;

    private void Awake()
    {
        btn_retry.onClick.AddListener(OnRestartButtonClick);
        btn_rank.onClick.AddListener(OnRankButtonClick);
        btn_home.onClick.AddListener(OnHomeButtonClick);
        EventCenter.AddListener(EventDefine.ShowGameOverPanel, Show);
        gameObject.SetActive(false);
    }
    private void OnDestroy()
    {
        EventCenter.RemoveListener(EventDefine.ShowGameOverPanel, Show);
    }
    private void Show()
    {
        //处理分数
        int curScore = GameManager.Instance.GetScore();//本局分数
        int maxScore = GameManager.Instance.GetMaxScore();//最高分数
        txt_score.text = curScore.ToString();
        if (curScore > maxScore)
        {
            img_new.gameObject.SetActive(true);
            txt_score_max.text = "最高分  " + curScore;
        }
        else
        {
            img_new.gameObject.SetActive(false);
            txt_score_max.text = "最高分  " + maxScore;
        }
        GameManager.Instance.SaveScore(curScore);

        //处理钻石
        txt_addDmdCount.text = "+" + GameManager.Instance.GetDiamondCnt().ToString();
        //更新总钻石数量
        GameManager.Instance.UpdateTotalDmdCnt(GameManager.Instance.GetDiamondCnt());
        gameObject.SetActive(true);
    }


    /// <summary>
    /// 再来一局按钮点击
    /// </summary>
    private void OnRestartButtonClick()
    {
        EventCenter.Broadcast(EventDefine.PlayClickBtn);
        StartCoroutine(ReloadScene());
        GameData.ifRetryGame = true;
    }

    /// <summary>
    /// 排行榜按钮点击
    /// </summary>
    private void OnRankButtonClick()
    {
        EventCenter.Broadcast(EventDefine.PlayClickBtn);
        EventCenter.Broadcast(EventDefine.ShowRankPanel);
    }

    /// <summary>
    /// 主界面按钮点击
    /// </summary>
    private void OnHomeButtonClick()
    {
        EventCenter.Broadcast(EventDefine.PlayClickBtn);
        StartCoroutine(ReloadScene());
        GameData.ifRetryGame = false;
    }

    IEnumerator ReloadScene()
    {
        yield return new WaitForSeconds(0.5f);
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}
