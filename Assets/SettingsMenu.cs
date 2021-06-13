using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;


public class SettingsMenu : MonoBehaviour
{
    public AudioMixer audioMixer;

    public void SetMVolume (float mvolume)
    {
        Debug.Log(mvolume);
        audioMixer.SetFloat("MasterVolume", mvolume);
    }

    public void SetSFXVolume (float sfxvolume)
    {
        Debug.Log(sfxvolume);
        audioMixer.SetFloat("SFXVolume", sfxvolume);
    }

    public void SetBGVolume (float bgvolume)
    {
        Debug.Log(bgvolume);
        audioMixer.SetFloat("BGVolume", bgvolume);
    }
}
