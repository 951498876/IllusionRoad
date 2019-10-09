using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// 管理资源常量
/// </summary>
/// 
[CreateAssetMenu(menuName ="CreateManagerVars")]
public class ManagerVars : ScriptableObject
{
    public List<Sprite> bgThemes = new List<Sprite>();//背景资源
    public List<Sprite> platformThemes = new List<Sprite>();//平台主题资源
    public List<Sprite> skins = new List<Sprite>();//商店皮肤资源
    public List<Sprite> characterSkins = new List<Sprite>();//角色皮肤资源
    public List<string> skinNames = new List<string>();//皮肤名称
    public List<int> skinPrices = new List<int>();//皮肤价格
    public float deltaX = 0.554f;
    public float deltaY = 0.645f;

    public GameObject basePlatform;//平台预制体
    public List<GameObject> commonPlatformGroups;//组合平台预制体-通用
    public List<GameObject> grassPlatformGroups;//组合平台预制体-草地
    public List<GameObject> winterPlatformGroups;//组合平台预制体-雪地
    public GameObject spikePlatformGroupL;//组合平台预制体-左钉
    public GameObject spikePlatformGroupR;//组合平台预制体-右钉

    public GameObject characterPrefab;//角色预制体
    public GameObject diamondPrefab;//角色预制体
    public GameObject deathEffect;//角色死亡特效
    public GameObject skinChoserPrefab;//皮肤选择项预制体

    public AudioClip jumpClip, fallClip, hitClip, diamondClip, btnClip;

    public Sprite musicOn, musicOff;

    public static ManagerVars GetManagerVars()
    {
        return Resources.Load<ManagerVars>("VarsContainer");
    }

}
