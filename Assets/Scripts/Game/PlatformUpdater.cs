using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// 平台自身确定内容
/// </summary>
public class PlatformUpdater : MonoBehaviour
{
    public SpriteRenderer[] spriteRdrs;
    public GameObject obstacle;
    private Rigidbody2D m_body;

    private bool startTimer;//是否开始掉落计时
    private float fallTime;//平台掉落时间

    private void Awake()
    {
        m_body = GetComponent<Rigidbody2D>();
    }

    /// <summary>
    /// 初始化平台数据
    /// </summary>
    /// <param name="sprite">平台样式</param>
    /// <param name="fallTime">掉落时间</param>
    public void Init(Sprite sprite,float fallTime)
    {
        m_body.bodyType = RigidbodyType2D.Static;
        this.fallTime = fallTime;
        startTimer = true;
        for (int i = 0; i < spriteRdrs.Length; i++)
        {
            spriteRdrs[i].sprite = sprite;
        }

        int obstacleDir = Random.Range(0, 2);
        if (obstacle != null)
        {
            if (obstacleDir == 0)//right
            {
                obstacle.transform.localPosition = new Vector3(-obstacle.transform.localPosition.x, obstacle.transform.localPosition.y, 0);
            }
        }
    }

    private void Update()
    {
        if (GameManager.Instance.IsGameStarted == false)
        {
            return;
        }
        if (startTimer && GameManager.Instance.IsPlayerMoved) //游戏开始&角色开始移动，开始计时掉落
        {
            fallTime -= Time.deltaTime;
            if (fallTime < 0)
            {
                startTimer = false;
                if (m_body.bodyType != RigidbodyType2D.Dynamic)
                {
                    m_body.bodyType = RigidbodyType2D.Dynamic;
                    StartCoroutine(DelayHide());
                }
                
            }
        }

        //掉落的平台离相机距离足够多时将其回收
        if (transform.position.y - Camera.main.transform.position.y < -6)
        {
            StartCoroutine(DelayHide());
        }
    }

    private IEnumerator DelayHide()
    {
        yield return new WaitForSeconds(1f);
        gameObject.SetActive(false);
    }
}
