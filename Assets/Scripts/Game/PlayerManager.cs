using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using UnityEngine.EventSystems;
/// <summary>
/// 人物角色管理
/// </summary>
public class PlayerManager : MonoBehaviour
{
    private bool IsDirLeft;//角色是否向左移动
    private bool IsJumping;//是否正在跳跃

    private Vector3 nextLPos;//角色下一个左平台位置
    private Vector3 nextRPos;//角色下一个右平台位置

    private ManagerVars vars;//系统资源管理
    private Rigidbody2D m_body;//角色刚体，检测跳跃结果
    private AudioSource m_audioSource;//角色音频系统

    public Transform rayDown, rayLeft, rayRight;//检测的射线

    public LayerMask platformLayer, obstacleLayer;//射线检测的对象层
    private SpriteRenderer renderer;

    private void Awake()
    {
        EventCenter.AddListener<int>(EventDefine.ChangeSkin, ChangeSkin);
        EventCenter.AddListener<bool>(EventDefine.IsMusicOn, IsMusicOn);
        vars = ManagerVars.GetManagerVars();
        m_body = GetComponent<Rigidbody2D>();
        renderer = GetComponent<SpriteRenderer>();
        m_audioSource = GetComponent<AudioSource>();
    }

    private void Start()
    {
        //进入游戏时默认选择之前选中的皮肤，需要读取记录此项的全局数据
        ChangeSkin(GameManager.Instance.GetSelectedSkin());
    }

    private void OnDestroy()
    {
        EventCenter.RemoveListener<int>(EventDefine.ChangeSkin, ChangeSkin);
        EventCenter.RemoveListener<bool>(EventDefine.IsMusicOn, IsMusicOn);
    }

    private void IsMusicOn(bool value)
    {
        m_audioSource.mute = !value;
    }

    /// <summary>
    /// 更换皮肤的调用
    /// </summary>
    /// <param name="skinIndex"></param>
    private void ChangeSkin(int skinIndex)
    {
        renderer.sprite = vars.characterSkins[skinIndex];
    }

    private bool IsPointerOverGameObject(Vector2 mousePosition)
    {
        //创建一个点击事件
        PointerEventData eventData = new PointerEventData(EventSystem.current);
        eventData.position = mousePosition;
        List<RaycastResult> res = new List<RaycastResult>();
        //向点击位置发射射线，检测是否点击到UI
        EventSystem.current.RaycastAll(eventData, res);
        return res.Count > 0;
    }

    void Update()
    {
        Debug.DrawRay(rayDown.position, Vector2.down * 1,Color.red);
        Debug.DrawRay(rayLeft.position, Vector2.left * 0.15f, Color.red);
        Debug.DrawRay(rayRight.position, Vector2.right * 0.15f, Color.red);

        if (IsPointerOverGameObject(Input.mousePosition))//选择UI控件，不进行角色事件
        {
            return;
        }

        if (!GameManager.Instance.IsGameStarted || GameManager.Instance.IsGameOver|| GameManager.Instance.IsGamePaused)//游戏未开始、暂停或结束，不进行角色事件
        {
            return;
        }

        if (Input.GetMouseButtonDown(0) && IsJumping == false && nextLPos != Vector3.zero) 
        {
            EventCenter.Broadcast(EventDefine.DecidePath);
            EventCenter.Broadcast(EventDefine.PlayerMove);
            IsJumping = true;
            m_audioSource.PlayOneShot(vars.jumpClip);
            Vector3 mousePos = Input.mousePosition;
            if (mousePos.x <= Screen.width / 2)
            {
                IsDirLeft = true;
            }
            else
            {
                IsDirLeft = false;
            }
            Jump();
        }

        //GameOver1.没落到平台
        if (m_body.velocity.y < 0)//角色正在下落
        {
            if (IsReachPlatform() == false && GameManager.Instance.IsGameOver == false)  
            {
                //游戏结束
                m_audioSource.PlayOneShot(vars.fallClip);
                renderer.sortingLayerName = "Default";
                GetComponent<BoxCollider2D>().enabled = false;
                GameManager.Instance.IsGameOver = true;

                StartCoroutine(DelayGameOver());
            }
        }

        //GameOver2.撞到障碍物了
        if (IsJumping)
        {
            if (IsReachObstacle() && GameManager.Instance.IsGameOver == false)
            {
                m_audioSource.PlayOneShot(vars.hitClip);
                GameObject go = ObjectPool.Instance.GetDeathEffect();
                go.SetActive(true);
                go.transform.position = transform.position;
                GameManager.Instance.IsGameOver = true;
                //Destroy(gameObject);不可销毁，销毁则协程失效
                renderer.enabled = false;

                StartCoroutine(DelayGameOver());

            }
        }

        //GameOver3.掉落了
        if (transform.position.y - Camera.main.transform.position.y < -6 && GameManager.Instance.IsGameOver == false)
        {
            m_audioSource.PlayOneShot(vars.fallClip);
            GameManager.Instance.IsGameOver = true;

            StartCoroutine(DelayGameOver());
            renderer.enabled = false;
            //gameObject.SetActive(false);

        }

    }

    IEnumerator DelayGameOver()
    {
        yield return new WaitForSeconds(1f);
        //调用结束面板
        EventCenter.Broadcast(EventDefine.ShowGameOverPanel);
    }


    private GameObject lastHitGo = null;//是否上一个计入总分的平台
    /// <summary>
    /// 是否落到平台
    /// </summary>
    /// <returns></returns>
    private bool IsReachPlatform()
    {
        RaycastHit2D hit = Physics2D.Raycast(rayDown.position, Vector2.down, 1f, platformLayer);
        //注：射线检测必须确定层级
        if (hit.collider != null)
        {
            if (hit.collider.tag == "Platform")
            {
                if (lastHitGo != hit.collider.gameObject)
                {
                    if (lastHitGo == null)
                    {
                        lastHitGo = hit.collider.gameObject;
                        return true;
                    }
                    else
                    {
                        EventCenter.Broadcast(EventDefine.AddScore);
                        lastHitGo = hit.collider.gameObject;
                    }
                }
                return true;
            }
        }
        return false;
    }
    /// <summary>
    /// 是否撞到障碍物
    /// </summary>
    /// <returns></returns>
    private bool IsReachObstacle()
    {
        RaycastHit2D leftHit = Physics2D.Raycast(rayLeft.position, Vector2.left, 0.15f, obstacleLayer);
        RaycastHit2D rightHit = Physics2D.Raycast(rayRight.position, Vector2.right, 0.15f, obstacleLayer);

        if (leftHit.collider != null)
        {
            if (leftHit.collider.tag == "Obstacle")
            {
                return true;
            }
        }
        if (rightHit.collider != null)
        {
            if (rightHit.collider.tag == "Obstacle")
            {
                return true;
            }
        }
        return false;
    }

    private void Jump()
    {
        //更换素材方向
        if (IsDirLeft)
        {
            transform.localScale = new Vector3(-1, 1, 1);
            //移动角色位置
            transform.DOMoveX(nextLPos.x, 0.2f);
            transform.DOMoveY(nextLPos.y + 0.8f, 0.15f);
        }
        else
        {
            transform.localScale = Vector3.one;
            //移动角色位置
            transform.DOMoveX(nextRPos.x, 0.2f);
            transform.DOMoveY(nextRPos.y + 0.8f, 0.15f);
        }

    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == "Platform")
        {
            IsJumping = false;

            Vector3 curPlatformPos = collision.gameObject.transform.position;
            nextLPos = new Vector3(curPlatformPos.x - vars.deltaX, curPlatformPos.y + vars.deltaY, 0);
            nextRPos = new Vector3(curPlatformPos.x + vars.deltaX, curPlatformPos.y + vars.deltaY, 0);

        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.collider.tag == "Item")
        {
            m_audioSource.PlayOneShot(vars.diamondClip);
            EventCenter.Broadcast(EventDefine.GetDiamond);
            collision.gameObject.SetActive(false);
        }
    }
}
