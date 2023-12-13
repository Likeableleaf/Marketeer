using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Audio;

public class GameUIManager : MonoBehaviour {
    // Variables
    public AudioMixer audioMixer;
    [SerializeField] private AudioControl Audio;
    [SerializeField] private static bool paused = false;
    [SerializeField] private GameObject obj_pauseMenu;
    [SerializeField] private GameObject cam;
    [SerializeField] private InGameUI obj_UI;
    [SerializeField] private Transform inGameCam;
    [SerializeField] private Transform menuGameCam;
    [SerializeField] private GameObject inGameUI;
    [SerializeField] private GameObject menuUI;
    [SerializeField] private GameObject ceiling;
    [SerializeField] private GameObject obj_endGame;
    [SerializeField] private GridManager gridManager;
    public bool runnin;
    public bool gameEnded = false;
    private float totalTime;

    private void Start() {
        MainMenu();
        //EndGame(); // for debug purposes
        Manager.OnEndGameUpdated.AddListener(endTheGame);
        totalTime = GridManager.GetTime();
    }
    
    // If esc is pressed pause game or resume based on if it is already or not
    void Update() {
        if (Input.GetKeyDown(KeyCode.Escape) && runnin) {
            if (paused) {
                Resume();
            } else {
                Pause();
            }
        }

        if(gameEnded)
        {
            EndGame();
        }

    }

    // Resume Game
    public void Resume () {
        obj_pauseMenu.SetActive(false);
        obj_UI.gameObject.SetActive(true);
        Time.timeScale = 1f;
        paused = false;
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
    }

    // Pause Game
    public void Pause () {
        obj_pauseMenu.SetActive(true);
        obj_UI.gameObject.SetActive(false);
        Time.timeScale = 0f;
        paused = true;
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
    }

    // Return to Main Menu
    public void QuitGame () {
        MainMenu();
        GridManager.SetCash(0f);
        GridManager.SetTime(0f);
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);

    }

    // Loads Main Menu
    public void MainMenu(){
        if (obj_endGame.activeInHierarchy)
        {
            obj_endGame.SetActive(false);
            endTheGame(false);

            // Set to Overworld cam if needed
            gridManager.ShiftDimension(2);
        }
        else
        {
            obj_pauseMenu.SetActive(false);
        }
        
        menuUI.SetActive(true);
        runnin = false;
        Camera.main.orthographicSize = 5;
        Time.timeScale = 0f;
        cam.transform.position = menuGameCam.position;
        cam.transform.rotation = menuGameCam.rotation;
        ceiling.SetActive(true);
    }
    
    // Starts game
    public void Play()
    {
        // change main camera + ui to in game, versions
        runnin = true;
        Time.timeScale = 1f;
        Camera.main.orthographicSize = 10;
        cam.transform.position = inGameCam.position;
        cam.transform.rotation = inGameCam.rotation;
        menuUI.SetActive(false);
        inGameUI.SetActive(true);
        ceiling.SetActive(false);
        GridManager.SetTime(0);
        //*/
        //SceneManager.LoadScene("Store");
    }

    // Ends Game
    public void EndGame()
    {
        obj_endGame.SetActive(true);
        obj_UI.gameObject.SetActive(false);
        GridManager.OnCashUpdated.Invoke(GridManager.GetCash());
        GridManager.OnTimeUpdated.Invoke(GridManager.GetTime());
        Time.timeScale = 0f;

        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
    }

    public void Settings(float Vol)
    {
        audioMixer.SetFloat("volume", Vol);
    }

    // Exits game
    public void Quit()
    {
        Application.Quit();
    }

    public void endTheGame(bool gameEnd)
    {
        gameEnded = gameEnd;
    }
}
