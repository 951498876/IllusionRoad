using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class GameData
{
    public static bool ifRetryGame = false;

    private bool isFirstGame;
    private bool isMusicOn;
    private int[] bestScoreArr;
    private int selectedSkin;
    private bool[] skinUnlocked;
    private int dmdCnt;

    //*********************数据设置***************************
    public void SetIsFirstGame(bool isFirstGame)
    {
        this.isFirstGame = isFirstGame;
    }
    public void SetIsMusicOn(bool isMusicOn)
    {
        this.isMusicOn = isMusicOn;
    }
    public void SetBestScoreArr(int[] bestScoreArr)
    {
        this.bestScoreArr = bestScoreArr;
    }
    public void SetSelectedSkin(int selectedSkin)
    {
        this.selectedSkin = selectedSkin;
    }
    public void SetSkinUnlocked(bool[] skinUnlocked)
    {
        this.skinUnlocked = skinUnlocked;
    }
    public void SetDmdCnt(int dmdCnt)
    {
        this.dmdCnt = dmdCnt;
    }

    //*********************数据获取***************************
    public bool GetIsFirstGame()
    {
        return isFirstGame;
    }
    public bool GetIsMusicOn()
    {
        return isMusicOn;
    }
    public int[] GetBestScoreArr()
    {
        return bestScoreArr;
    }
    public int GetSelectedSkin()
    {
        return selectedSkin;
    }
    public bool[] GetSkinUnlocked()
    {
        return skinUnlocked;
    }
    public int GetDmdCnt()
    {
        return dmdCnt;
    }
}
