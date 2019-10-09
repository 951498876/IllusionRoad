using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// 控制相机跟随角色移动
/// </summary>
public class CameraFollow : MonoBehaviour
{
    private Transform target;//相机跟踪的目标

    private Vector3 offset;//相机与目标偏移量

    private Vector2 velocity;

    void Update()
    {
        if (target == null && GameObject.FindGameObjectWithTag("Player") != null)
        {
            target = GameObject.FindGameObjectWithTag("Player").transform;
            offset = target.position - transform.position;
        }
    }
    private void FixedUpdate()
    {
        print("target:"+target == null);
        if (target != null)
        {
            float posX = Mathf.SmoothDamp(transform.position.x, target.position.x - offset.x, ref velocity.x, 0.05f);
            float posY = Mathf.SmoothDamp(transform.position.y, target.position.y - offset.x, ref velocity.y, 0.05f);

            if (posY > transform.position.y)
            {
                transform.position = new Vector3(posX, posY, transform.position.z);
            }
        }
    }
}
