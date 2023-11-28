using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class scr_MainMenu : MonoBehaviour
{
    // Variable Cache
    [SerializeField] private Camera menuCam;
    [SerializeField] private Camera inGameCam;
    [SerializeField] private GameObject inGameUI;
    [SerializeField] private GameObject menuUI;
    [SerializeField] private GameObject ceiling;

    private void Start() {
        inGameCam = Camera.main;
        menuCam.enabled = true;
        inGameCam.enabled = false;
        ceiling.SetActive(true);
        
    }

    // Starts game
    public void Play()
    {
        // change main camera + ui to in game, versions
        
        inGameCam.enabled = true;
        menuUI.SetActive(false);
        inGameUI.SetActive(true);
        ceiling.SetActive(false);
        menuCam.enabled = false;
        //*/
        //SceneManager.LoadScene("Store");
    }

    // Exits game
    public void Quit()
    {
        Application.Quit();
    }
}
