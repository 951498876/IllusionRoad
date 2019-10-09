using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClickAudio : MonoBehaviour
{
    private AudioSource m_audioSource;
    private ManagerVars vars;
    private void Awake()
    {
        m_audioSource = GetComponent<AudioSource>();
        vars = ManagerVars.GetManagerVars();
        EventCenter.AddListener(EventDefine.PlayClickBtn, PlayAudio);
        EventCenter.AddListener<bool>(EventDefine.IsMusicOn, IsMusicOn);
    }
    private void OnDestroy()
    {
        EventCenter.RemoveListener(EventDefine.PlayClickBtn, PlayAudio);
        EventCenter.RemoveListener<bool>(EventDefine.IsMusicOn, IsMusicOn);
    }
    private void PlayAudio()
    {
        m_audioSource.PlayOneShot(vars.btnClip);
    }

    private void IsMusicOn(bool value)
    {
        m_audioSource.mute = !value;
    }
}
