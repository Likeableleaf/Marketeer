using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Audio;
using UnityEngine.UI;
using Scene = UnityEngine.SceneManagement.Scene;
public class GameManager
{

    private static GameManager _instance;

    [HideInInspector]
    public static GameManager instance
    {
        get
        {
            if (_instance == null)
            {
                Debug.Log("Game Manager created");
                _instance = new GameManager();
            }
            else if (_instance != null)
            {
                Debug.Log("Instance exists");
            }
            return _instance;
        }
        set
        {
            _instance = value;
        }
    }

    //Audio
    public float AudioSetting = 1;
    public float AudioSlider = 1;
    public AudioMixer audioMasterMixer;
    private GameManager()
    {

    }

    //Set/Get Audio setting

    /*When opening a new scene: 
     * GameManager.setAudioMixerMaster(audioMasterMixer);
     * GameManager.setAudioSetting(GameManager.getAudioSetting());
     */
    public void setAudioSetting(float volume)
    {
        audioMasterMixer.SetFloat("volume", volume);
        AudioSetting = volume;
        Debug.Log("Volume is at: " + volume);
    }

    public void setAudioSlider(float sliderVol)
    {
        AudioSlider = sliderVol;
    }

    public float getAudioSetting()
    {
        return AudioSetting;
    }

    public float getAudioSlider()
    {
        return AudioSlider;
    }

    public void setAudioMixerMaster(AudioMixer Master)
    {
        audioMasterMixer = Master;
    }
}
