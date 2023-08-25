using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class scr_MainMenu : MonoBehaviour
{
    // Starts game
    public void Play()
    {
        SceneManager.LoadScene("SampleScene");
    }

    // Exits game
    public void Quit()
    {
        Application.Quit();
    }
}
