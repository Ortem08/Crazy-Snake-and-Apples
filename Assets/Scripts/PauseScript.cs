using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseScript : MonoBehaviour
{
    public static bool GameIsPaused;
    public GameObject PauseMenuUI;
    
    //Объект игрока, чтобы во время паузы не реагировал на мышь
    public GameObject player;

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (GameIsPaused)
                Resume();
            else
                Pause();
        }
    }

    public void Resume()
    {
        PauseMenuUI.SetActive(false); 
        player.GetComponent<FirstPersonController>().enabled = true;
        Time.timeScale = 1f;
        GameIsPaused = false;


    }

    void Pause()
    {
        PauseMenuUI.SetActive(true); 
        player.GetComponent<FirstPersonController>().enabled = false;
        Time.timeScale = 0f;
        GameIsPaused = true;
    }

    public void LoadMainMenu()
    {
        GameIsPaused = false;
        Time.timeScale = 1f;
        SceneManager.LoadScene("Menu");
    }

    public void QuitGame()
    {
        Application.Quit();
    }
}
