using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class scr_MainMenu : MonoBehaviour
{
    // Variable Cache
    [SerializeField] private GameObject cam;
    [SerializeField] private Transform inGameCam;
    [SerializeField] private Transform menuGameCam;
    [SerializeField] private GameObject inGameUI;
    [SerializeField] private GameObject menuUI;
    [SerializeField] private GameObject ceiling;

    private void Start() {
        cam.transform.position = menuGameCam.position;
        cam.transform.rotation = menuGameCam.rotation;
        ceiling.SetActive(true);
    }

    // Starts game
    public void Play()
    {
        // change main camera + ui to in game, versions
        
        cam.transform.position = inGameCam.position;
        cam.transform.rotation = inGameCam.rotation;
        menuUI.SetActive(false);
        inGameUI.SetActive(true);
        ceiling.SetActive(false);
        //*/
        //SceneManager.LoadScene("Store");
    }

    // Exits game
    public void Quit()
    {
        Application.Quit();
    }
}
