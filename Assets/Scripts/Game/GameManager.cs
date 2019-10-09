using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;
using System.Linq;

/// <summary>
/// 控制游戏开始结束
/// </summary>
public class GameManager : MonoBehaviour
{
    public static GameManager Instance;
    private GameData data;
    private ManagerVars vars;

    /// <summary>
    /// 游戏是否开始
    /// </summary>
    public bool IsGameStarted { get; set; }
    /// <summary>
    /// 游戏是否暂停
    /// </summary>
    public bool IsGamePaused { get; set; }
    /// <summary>
    /// 游戏是否结束
    /// </summary>
    public bool IsGameOver { get; set; }
    /// <summary>
    /// 角色是否移动
    /// </summary>
    public bool IsPlayerMoved { get; set; }

    private int gameScore;//当前局游戏成绩
    private int gameDiamond;//当前局游戏钻石数

    //*******GameData全局数据********
    private bool isFirstGame;
    private bool isMusicOn;
    private int[] bestScoreArr;
    private int selectedSkin;
    private bool[] skinUnlocked;
    private int dmdCnt;
    //*******************************

    private void Awake()
    {
        vars = ManagerVars.GetManagerVars();
        Instance = this;

        IsGameOver = false;
        IsPlayerMoved = false;

        EventCenter.AddListener(EventDefine.AddScore, AddScore);
        EventCenter.AddListener(EventDefine.GetDiamond, GetDiamond);
        EventCenter.AddListener(EventDefine.PlayerMove, PlayerMove);

        if (GameData.ifRetryGame)
        {
            IsGameStarted = true;//通过再来一次重新加载场景的话，直接设置游戏开始
        }

        InitGameData();
    }

    private void OnDestroy()
    {
        EventCenter.RemoveListener(EventDefine.AddScore, AddScore);
        EventCenter.RemoveListener(EventDefine.GetDiamond, GetDiamond);
        EventCenter.RemoveListener(EventDefine.PlayerMove, PlayerMove);
    }
    
    /// <summary>
    /// 增加游戏成绩
    /// </summary>
    private void AddScore()
    {
        if (!IsGameStarted || IsGamePaused || IsGameOver)
        {
            return;
        }
        gameScore++;
        EventCenter.Broadcast(EventDefine.UpdateScoreOfPanel, gameScore);
    }

    /// <summary>
    /// 保存成绩
    /// </summary>
    public void SaveScore(int score)
    {
        List<int> scoreList = bestScoreArr.ToList();
        scoreList.Sort((x, y) => (-x.CompareTo(y)));//从大到小进行排序
        bestScoreArr = scoreList.ToArray();

        if (bestScoreArr[2] < score)
        {
            bestScoreArr[2] = score;
            scoreList = bestScoreArr.ToList();
            scoreList.Sort((x, y) => (-x.CompareTo(y)));
            bestScoreArr = scoreList.ToArray();
        }
    }

    /// <summary>
    /// 获取最高分
    /// </summary>
    /// <returns></returns>
    public int GetMaxScore()
    {
        return bestScoreArr[0];
    }

    /// <summary>
    /// 获取记录
    /// </summary>
    /// <returns></returns>
    public int[] GetScoreArr()
    {
        List<int> scoreList = bestScoreArr.ToList();
        scoreList.Sort((x, y) => (-x.CompareTo(y)));//从大到小进行排序
        bestScoreArr = scoreList.ToArray();

        return bestScoreArr;
    }

    /// <summary>
    /// 增加钻石
    /// </summary>
    private void GetDiamond()
    {
        gameDiamond++;
        EventCenter.Broadcast(EventDefine.UpdateDmdOfPanel, gameDiamond);
    }


    /// <summary>
    /// 获取总钻石数
    /// </summary>
    public int GetTotalDmdCnt()
    {
        return dmdCnt;
    }

    /// <summary>
    /// 更新总钻石数量
    /// </summary>
    /// <param name="value"></param>
    public void UpdateTotalDmdCnt(int value)
    {
        dmdCnt += value;
        Save();
    }

    /// <summary>
    /// 玩家开始移动
    /// </summary>
    private void PlayerMove()
    {
        IsPlayerMoved = true;
    }


    /// <summary>
    /// 获取分数
    /// </summary>
    /// <returns></returns>
    public int GetScore()
    {
        return gameScore;
    }

    /// <summary>
    /// 获取钻石数
    /// </summary>
    /// <returns></returns>
    public int GetDiamondCnt()
    {
        return gameDiamond;
    }


    /// <summary>
    /// 获取当前皮肤是否解锁
    /// </summary>
    /// <param name="index"></param>
    /// <returns></returns>
    public bool GetIfSkinUnlocked(int index)
    {
        if (index >= 0 && index < skinUnlocked.Length)
        {
            return skinUnlocked[index];
        }
        return false;
    }


    /// <summary>
    /// 设置当前皮肤解锁
    /// </summary>
    /// <param name="index"></param>
    /// <returns></returns>
    public void SetSkinUnlocked(int index)
    {
        skinUnlocked[index] = true;
        Save();
    }

    /// <summary>
    /// 更换皮肤
    /// </summary>
    /// <param name="skinIndex"></param>

    public void SetSelectedSkin(int skinIndex)
    {
        selectedSkin = skinIndex;
        Save();
    }

    /// <summary>
    /// 获取当前选择的皮肤
    /// </summary>
    /// <returns></returns>
    public int GetSelectedSkin()
    {
        return selectedSkin;
    }

    /// <summary>
    /// 设置音效是否开启
    /// </summary>
    /// <param name="val"></param>
    public void SwitchIsMusicOn()
    {
        isMusicOn = !isMusicOn;
        Save();
    }

    /// <summary>
    /// 获取音效是否开启
    /// </summary>
    public bool GetIsMusicOn()
    {
        return isMusicOn;
    }

    //*********************数据初始***************************
    private void InitGameData()
    {
        Read();
        if (data != null)
        {
            isFirstGame = data.GetIsFirstGame();
        }
        else//没读到数据，说明是第一次游戏
        {
            isFirstGame = true;
        }

        if (isFirstGame)
        {
            isFirstGame = false;//置空，避免下次重复赋初值
            isMusicOn = true;
            bestScoreArr = new int[3];
            selectedSkin = 0;
            skinUnlocked = new bool[vars.skins.Count];
            skinUnlocked[0] = true;
            dmdCnt = 5;

            data = new GameData();
            Save();
        }
        else
        {
            isMusicOn = data.GetIsMusicOn();
            bestScoreArr = data.GetBestScoreArr();
            selectedSkin = data.GetSelectedSkin();
            skinUnlocked = data.GetSkinUnlocked();
            dmdCnt = data.GetDmdCnt();
        }
    }


    //*********************数据保存***************************
    /// <summary>
    /// 保存全局数据
    /// </summary>
    private void Save()
    {
        try
        {
            BinaryFormatter bf = new BinaryFormatter();
            using (FileStream fs = File.Create(Application.persistentDataPath + "/GameData.dat"))//使用using不需要用close进行关闭
            {
                data.SetIsFirstGame(isFirstGame);
                data.SetIsMusicOn(isMusicOn);
                data.SetBestScoreArr(bestScoreArr);
                data.SetSelectedSkin(selectedSkin);
                data.SetSkinUnlocked(skinUnlocked);
                data.SetDmdCnt(dmdCnt);
                bf.Serialize(fs, data);
            }
        }
        catch (System.Exception e)
        {
            Debug.Log(e.Message);
        }
    }

    //*********************数据读取***************************
    /// <summary>
    /// 读取全局数据
    /// </summary>
    private void Read()
    {
        try
        {
            BinaryFormatter bf = new BinaryFormatter();
            using (FileStream fs = File.OpenRead(Application.persistentDataPath + "/GameData.dat"))//使用using不需要用close进行关闭
            {
                data = (GameData)bf.Deserialize(fs);
            }
        }
        catch (System.Exception e)
        {
            Debug.Log(e.Message);
        }
    }
    //*********************数据重置***************************
    /// <summary>
    /// 重置数据
    /// </summary>
    public void ResetData()
    {
        isFirstGame = false;
        isMusicOn = true;
        bestScoreArr = new int[3];
        selectedSkin = 0;
        skinUnlocked = new bool[vars.skins.Count];
        skinUnlocked[0] = true;
        dmdCnt = 5;

        Save();
    }
}
