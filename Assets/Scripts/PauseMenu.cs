using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine.SceneManagement;
using UnityEngine;

public class PauseMenu : MonoBehaviour
{
    public static bool GameIsPaused = false;

    public GameObject background;
    public GameObject pauseMenu;
    public GameObject pauseOptionMenu;
	public GameObject player;

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (GameIsPaused)
            {
                Resume();
            }
            else 
            {
                Pause();
            }
        }
    }

    public void Resume()
    {
        background.SetActive(false);
        pauseMenu.SetActive(false);
        pauseOptionMenu.SetActive(false);
        Time.timeScale = 1f;
        GameIsPaused = false;

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        
		player.GetComponent<PlayerPortalShoot>().ExtUpdatePortals();
    }
    
    void Pause()
    {
        background.SetActive(true);
        pauseMenu.SetActive(true);
        pauseOptionMenu.SetActive(false);
        Time.timeScale = 0f;
        GameIsPaused = true;

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    public void LoadMenu()
    {
        pauseMenu.SetActive(false);
        pauseMenu.SetActive(false);
        pauseOptionMenu.SetActive(false);
        Time.timeScale = 1f;
        GameIsPaused = false;

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        SceneManager.LoadScene("MainMenu");

    }
}