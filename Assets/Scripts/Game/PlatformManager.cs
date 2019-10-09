using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// 控制平台的生成
/// </summary>
/// 

public enum GroupTheme
{
    Grass,
    Winter,
}

public class PlatformManager : MonoBehaviour
{

    public Vector3 startPos;//初始位置

    private int spawnCount;//生成数量
    private Vector3 spawnPos;//生成位置
    private bool isSpawnLeft;//生成方向
    private Sprite platformTheme;//平台主题
    private GroupTheme groupTheme;//组合平台主题

    private int spawnCountAfterSpike;//钉刺后续平台生成数量
    private Vector3 spawnPosAfterSpike;//钉刺后续平台生成位置
    private bool isSpawnAfterSpikeLeft;//钉刺后续平台生成方向

    private ManagerVars vars;

    private void Awake()
    {
        vars = ManagerVars.GetManagerVars();
        EventCenter.AddListener(EventDefine.DecidePath, DecidePath);
    }

    private void OnDestroy()
    {
        EventCenter.RemoveListener(EventDefine.DecidePath, DecidePath);
    }

    private void Start()
    {
        RandomSpawnTheme();
        spawnPos = startPos;
        isSpawnLeft = false;


        //生成平台
        spawnCount = 5;
        for (int i = 0; i < 4; i++)
        {
            DecidePath();
        }

        //生成人物
        GameObject go = Instantiate(vars.characterPrefab);
        go.transform.position = new Vector3(0, -1.8f, 0);

    }
    private void Update()
    {
        if (GameManager.Instance.IsGameStarted && GameManager.Instance.IsGameOver == false) 
        {
            UpdateFallTime();
        }
    }

    public int scoreLevel = 10;//分数等级
    public float fallTime;//掉落时间
    public float minFallTime;//最小掉落时间
    public float param;//增强系数
    /// <summary>
    /// 更新平台掉落时间
    /// </summary>
    private void UpdateFallTime()
    {
        if (GameManager.Instance.GetScore() > scoreLevel)
        {
            scoreLevel *= 2;
            fallTime *= param;
            if (fallTime < minFallTime)
            {
                fallTime = minFallTime;
            }
        }
    }

    /// <summary>
    /// 确定路径,生成平台
    /// </summary>
    private void DecidePath()
    {
        if (spawnCountAfterSpike > 0)//生成了钉刺平台，需要生成后续平台
        {
            //钉刺后续平台起始位置
            spawnCountAfterSpike--;
            SpawnPlatformAfterSpike();
        }

        if (spawnCount > 0)
        {
            spawnCount--;
        }
        else
        {
            isSpawnLeft = !isSpawnLeft;
            spawnCount = Random.Range(1, 4);
        }
        SpawnPlatform();
    }
    /// <summary>
    /// 随机要生成的平台主题
    /// </summary>
    private void RandomSpawnTheme()
    {
        int ran = Random.Range(0, vars.platformThemes.Count);
        platformTheme = vars.platformThemes[ran];

        if (ran == 2)
        {
            groupTheme = GroupTheme.Winter;
        }
        else
        {
            groupTheme = GroupTheme.Grass;
        }
    }

    /// <summary>
    /// 生成平台
    /// </summary>
    private void SpawnPlatform()
    {
        if (spawnCount == 0)
        {
            //生成组合平台
            int ranType = Random.Range(0, 3);
            switch (ranType)
            {
                case 0:
                    //生成通用组合平台
                    SpawnCommonGroupPlatform();
                    break;
                case 1:
                    //生成主题组合平台
                    switch (groupTheme)
                    {
                        case GroupTheme.Grass:
                            SpawnGrassGroupPlatform();
                            break;
                        case GroupTheme.Winter:
                            SpawnWinterGroupPlatform();
                            break;
                        default:break;
                    }                    
                    break;
                case 2:
                    //生成钉子组合平台
                    spawnCountAfterSpike = 6;
                    SpawnSpikeGroupPlatform(!isSpawnLeft);
                    
                    //为了生成钉刺平台的后续平台，确认起始位置与方向
                    spawnPosAfterSpike = new Vector3(spawnPos.x + (isSpawnLeft ? 1 : -1) * vars.deltaX * 3, spawnPos.y + vars.deltaY, 0);
                    isSpawnAfterSpikeLeft = !isSpawnLeft;
                    break;
                default:break;
            }
        }
        else
        {
            //生成普通平台
            SpawnBasePlatform();
        }

        //角色开始移动后，在上方生成的平台上随机生成钻石
        int ranSpawnDiamond = Random.Range(0, 10);
        if (ranSpawnDiamond == 6 && GameManager.Instance.IsPlayerMoved)
        {
            GameObject go = ObjectPool.Instance.GetDiamond();
            go.transform.position = new Vector3(spawnPos.x, spawnPos.y + 0.5f, 0);
            go.SetActive(true);
        }
        
        //更新生成后下个平台的位置
        if (isSpawnLeft)
        {
            spawnPos = new Vector3(spawnPos.x - vars.deltaX, spawnPos.y + vars.deltaY, 0);
        }
        else
        {
            spawnPos = new Vector3(spawnPos.x + vars.deltaX, spawnPos.y + vars.deltaY, 0);
        }
    }

    /// <summary>
    /// 生成钉刺平台后续平台
    /// </summary>
    private void SpawnPlatformAfterSpike()
    {
        GameObject go = ObjectPool.Instance.GetBaseFromPool();
        go.transform.position = spawnPosAfterSpike;
        //Debug.Log(string.Format("生成平台，位置：{0},{1},数量{2}", spawnPosAfterSpike.x, spawnPosAfterSpike.y, spawnCountAfterSpike));
        go.GetComponent<PlatformUpdater>().Init(platformTheme, fallTime);
        go.SetActive(true);

        spawnPosAfterSpike = new Vector3(spawnPosAfterSpike.x + (isSpawnAfterSpikeLeft ? -1 : 1) * vars.deltaX, spawnPos.y + vars.deltaY, 0);
    }


    /// <summary>
    /// 生成普通平台
    /// </summary>
    private void SpawnBasePlatform()
    {
        GameObject go = ObjectPool.Instance.GetBaseFromPool();
        go.transform.position = spawnPos;

        go.GetComponent<PlatformUpdater>().Init(platformTheme, fallTime);
        go.SetActive(true);
    }
    /// <summary>
    /// 生成通用组合平台
    /// </summary>
    private void SpawnCommonGroupPlatform()
    {
        GameObject go = ObjectPool.Instance.GetCommonFromPool();
        go.transform.position = spawnPos;

        go.GetComponent<PlatformUpdater>().Init(platformTheme, fallTime);
        go.SetActive(true);
    }
    /// <summary>
    /// 生成草地组合平台
    /// </summary>
    private void SpawnGrassGroupPlatform()
    {
        GameObject go = ObjectPool.Instance.GetGrassFromPool();
        go.transform.position = spawnPos;

        go.SetActive(true);
        go.GetComponent<PlatformUpdater>().Init(platformTheme, fallTime);
    }
    /// <summary>
    /// 生成冬天组合平台
    /// </summary>
    private void SpawnWinterGroupPlatform()
    {
        GameObject go = ObjectPool.Instance.GetWinterFromPool();
        go.transform.position = spawnPos;

        go.GetComponent<PlatformUpdater>().Init(platformTheme, fallTime);
        go.SetActive(true);
    }

    /// <summary>
    /// 生成钉刺组合平台
    /// </summary>
    private void SpawnSpikeGroupPlatform(bool isSpawnRight)
    {
        GameObject spikePlatform = isSpawnRight ? ObjectPool.Instance.GetLSpikeFromPool() : ObjectPool.Instance.GetRSpikeFromPool();
        //GameObject go = Instantiate(spikePlatform, transform);
        spikePlatform.transform.position = spawnPos;

        spikePlatform.GetComponent<PlatformUpdater>().Init(platformTheme, fallTime);
        spikePlatform.SetActive(true);
    }
}
