using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class AudioControl : MonoBehaviour
{

    [SerializeField] private Manager manager;
    [SerializeField] private float volume = 0.5f;
    [SerializeField] private AudioSource PassiveMusic;
    [SerializeField] private AudioSource ChaseMusic;
    [SerializeField] private AudioClip AngeryNoise;
    [SerializeField] private AudioClip AngeryNoise2;
    public AudioMixer audio;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    public void Update()
    {
        
        //if the manager is in any state other than chasing the player, play mall music
        if(manager.getState() != Manager.ManagerState.ChasePlayer)
        {
            //Stop chase music
            if (ChaseMusic.isPlaying)
            {
                ChaseMusic.Stop();
            }
            //start passive music
            if (!PassiveMusic.isPlaying)
            {
                PassiveMusic.volume = volume;
                PassiveMusic.Play();
                
            }
            
        }
        else //the manager is chasing the player
        {
            //Stop passive music
            if (PassiveMusic.isPlaying)
            {
                PassiveMusic.Stop();
            }
            //start passive music
            if (!ChaseMusic.isPlaying)
            {
                
                ChaseMusic.volume = volume;
                //generate a random number for which angrynoise to play
                float number = Random.Range(0, 1);
                if (number > 0.5f)
                {
                    ChaseMusic.PlayOneShot(AngeryNoise, volume);
                }
                else
                {
                    ChaseMusic.PlayOneShot(AngeryNoise2, volume);
                }
                //play chase music
                ChaseMusic.Play();
                
            } //end if
           
        }// end else
    }// end update


    public void setVolume(float Vol)
    {
        volume = Vol;
    }

}// end class
