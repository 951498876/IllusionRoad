using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectPool : MonoBehaviour
{
    public static ObjectPool Instance;

    public int initSpawnCount = 5;
    private List<GameObject> basePlatformList = new List<GameObject>();//素体平台
    private List<GameObject> commonPlatformList = new List<GameObject>();//通用组合平台
    private List<GameObject> winterPlatformList = new List<GameObject>();//冰雪组合平台
    private List<GameObject> grassPlatformList = new List<GameObject>();//草地组合平台
    private List<GameObject> lSpikePlatformList = new List<GameObject>();//左钉组合平台
    private List<GameObject> rSpikePlatformList = new List<GameObject>();//右钉组合平台

    private List<GameObject> diamondPrefabList = new List<GameObject>();//右钉组合平台

    private List<GameObject> deathEffectList = new List<GameObject>();//死亡特效

    private ManagerVars vars;

    private void Awake()
    {
        Instance = this;
        vars = ManagerVars.GetManagerVars();
        Init();
    }

    private void Init()
    {
        for (int i = 0; i < initSpawnCount; i++)
        {
            InstantiateObjects(vars.basePlatform, ref basePlatformList);
            InstantiateObjects(vars.commonPlatformGroups, ref commonPlatformList);
            InstantiateObjects(vars.grassPlatformGroups, ref grassPlatformList);
            InstantiateObjects(vars.spikePlatformGroupL, ref lSpikePlatformList);
            InstantiateObjects(vars.spikePlatformGroupR, ref rSpikePlatformList);

            InstantiateObjects(vars.diamondPrefab, ref diamondPrefabList); 

            InstantiateObjects(vars.deathEffect, ref deathEffectList);
        }
    }

    /// <summary>
    /// 实例化并加入对象池
    /// </summary>
    /// <param name="prefab">预制体对象</param>
    /// <param name="goList">对象列表</param>
    private GameObject InstantiateObjects(GameObject prefab, ref List<GameObject> goList)
    {
        GameObject go = Instantiate(prefab, transform);
        go.SetActive(false);
        goList.Add(go);
        return go;
    }
    /// <summary>
    /// 实例化并加入对象池
    /// </summary>
    /// <param name="prefabs">预制体对象</param>
    /// <param name="goList">对象列表</param>
    private void InstantiateObjects(List<GameObject> prefabs, ref List<GameObject> goList)
    {
        for (int i = 0; i < prefabs.Count; i++)
        {
            GameObject go = Instantiate(prefabs[i], transform);
            go.SetActive(false);
            goList.Add(go);
        }
    }


    /// <summary>
    /// 从线程池获取固定平台(特效、钻石等预制体),内容固定
    /// </summary>
    /// <returns></returns>
    private GameObject GetFixedPlatformFromPool(ref List<GameObject> poolList, GameObject var)
    {
        for (int i = 0; i < poolList.Count; i++)
        {
            if (poolList[i].activeInHierarchy == false)
            {
                return poolList[i];
            }
        }
        GameObject go = InstantiateObjects(var, ref poolList);
        return go;
    }

    /// <summary>
    /// 从线程池获取组合平台,平台内容多种组合
    /// </summary>
    /// <param name="poolList"></param>
    /// <param name="varList"></param>
    /// <returns></returns>
    private GameObject GetGroupPlatformFromPool(ref List<GameObject> poolList, List<GameObject> varList)
    {
        for (int i = 0; i < poolList.Count; i++)
        {
            if (poolList[i].activeInHierarchy == false)
            {
                return poolList[i];
            }
        }
        int ran = Random.Range(0, varList.Count);
        return InstantiateObjects(varList[ran], ref poolList);
    }
    /// <summary>
    /// 获取基本平台
    /// </summary>
    /// <returns></returns>
    public GameObject GetBaseFromPool()
    {
        return GetFixedPlatformFromPool(ref basePlatformList, vars.basePlatform);
    }
    /// <summary>
    /// 获取通用组合平台
    /// </summary>
    /// <returns></returns>
    public GameObject GetCommonFromPool()
    {
        return GetGroupPlatformFromPool(ref commonPlatformList, vars.commonPlatformGroups);
    }
    /// <summary>
    /// 获取冰雪组合平台
    /// </summary>
    /// <returns></returns>
    public GameObject GetWinterFromPool()
    {
        return GetGroupPlatformFromPool(ref winterPlatformList, vars.winterPlatformGroups);
    }
    /// <summary>
    /// 获取草地组合平台
    /// </summary>
    /// <returns></returns>
    public GameObject GetGrassFromPool()
    {
        return GetGroupPlatformFromPool(ref grassPlatformList, vars.grassPlatformGroups);
    }
    /// <summary>
    /// 获取左钉刺组合平台
    /// </summary>
    /// <returns></returns>
    public GameObject GetLSpikeFromPool()
    {
        return GetFixedPlatformFromPool(ref lSpikePlatformList, vars.spikePlatformGroupL);
    }
    /// <summary>
    /// 获取右钉刺组合平台
    /// </summary>
    /// <returns></returns>
    public GameObject GetRSpikeFromPool()
    {
        return GetFixedPlatformFromPool(ref rSpikePlatformList, vars.spikePlatformGroupR);
    }

    /// <summary>
    /// 获取钻石
    /// </summary>
    /// <returns></returns>
    public GameObject GetDiamond()
    {
        return GetFixedPlatformFromPool(ref diamondPrefabList, vars.diamondPrefab);
    }

    /// <summary>
    /// 获取死亡特效
    /// </summary>
    /// <returns></returns>
    public GameObject GetDeathEffect()
    {
        return GetFixedPlatformFromPool(ref deathEffectList, vars.deathEffect);
    }
}
