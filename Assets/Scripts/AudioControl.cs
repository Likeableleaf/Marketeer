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
    [SerializeField] private GameObject endGame;
    public AudioMixer audioMasterMixer;
    private GameManager GameManager;
    // Start is called before the first frame update
    void Start()
    {
        GameManager = GameManager.instance;

        GameManager.setAudioMixerMaster(audioMasterMixer);
        
    }

    // Update is called once per frame
    public void Update()
    {

        //if the manager is in any state other than chasing the player, play mall music
        if (manager.getState() != Manager.ManagerState.ChasePlayer && endGame.activeSelf == false && (manager.getStrikerCount() < 2))
        {
            //Stop chase music
            if (ChaseMusic.isPlaying)
            {
                ChaseMusic.Stop();
            }
            //start passive music
            if (!PassiveMusic.isPlaying)
            {
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
            //start chase music
            if (!ChaseMusic.isPlaying)
            {
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


        if (!ChaseMusic.isPlaying && !PassiveMusic.isPlaying)
        {

            PassiveMusic.Play();

        }

    }// end update

}// end class
