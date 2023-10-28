using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class scr_PauseMenu : MonoBehaviour {
    // Variables
    [SerializeField] private static bool paused = false;
    [SerializeField] private GameObject obj_pauseMenu;
    [SerializeField] private GameObject obj_UI;

    // If esc is pressed pause game or resume based on if it is already or not
    void Update() {
        if (Input.GetKeyDown(KeyCode.Escape)) {
            if (paused) {
                Resume();
            } else {
                Pause();
            }
        }
    }

    // Resume Game
    public void Resume () {
        obj_pauseMenu.SetActive(false);
        obj_UI.SetActive(true);
        Time.timeScale = 1f;
        paused = false;
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
    }

    // Pause Game
    public void Pause () {
        obj_pauseMenu.SetActive(true);
        obj_UI.SetActive(false);
        Time.timeScale = 0f;
        paused = true;
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
    }

    // Return to Main Menu
    public void QuitGame () {
        Time.timeScale = 1f;
        SceneManager.LoadScene("MainMenu");
    }
}
