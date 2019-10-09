using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BgTheme : MonoBehaviour
{
    private SpriteRenderer m_spriteRenderer;

    private ManagerVars vars;
    public void Awake()
    {
        vars = ManagerVars.GetManagerVars();
        m_spriteRenderer = transform.GetComponent<SpriteRenderer>();

        int ranVal = Random.Range(0, vars.bgThemes.Count);
        m_spriteRenderer.sprite = vars.bgThemes[ranVal];
    }
}
