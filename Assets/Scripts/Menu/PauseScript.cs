using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class PauseScript : MonoBehaviour
{
    public bool GameIsPaused => statesStack.Peek() == PauseStates.GeneralPause;
    public bool CanCloseInventory => statesStack.Peek() == PauseStates.InventoryPause;
    
    public GameObject PauseMenuUI;
    [SerializeField]
    public GameObject settingsUI;
    
        
    private GameObject player;
    
    private Stack<PauseStates> statesStack;
    private PlayerUI playerUI;

    private void Awake()
    {
        player = GameObject.FindGameObjectWithTag("Player");
        
        statesStack = new Stack<PauseStates>();
        statesStack.Push(PauseStates.None);
        
        playerUI = FindObjectOfType<PlayerUI>();
        playerUI.OnOpenInventory.AddListener(OnOpenInventory);
        playerUI.OnCloseInventory.AddListener(OnCloseInventory);
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (settingsUI.activeSelf)
            {
                settingsUI.SetActive(false);
                PauseMenuUI.SetActive(true);
            }
            else if (GameIsPaused)
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
        if (GameIsPaused)
        {
            statesStack.Pop();
        }
        
        PauseMenuUI.SetActive(false);
        
        if (statesStack.Peek() == PauseStates.InventoryPause)
        {
            return;
        }
        
        Debug.Log($"LAST STATE IS {statesStack.Peek()}");
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
        SingletonInputManager.instance.InputMap.Enable();
        player.SetActive(true);
        Time.timeScale = 1f;
    }

    public void Pause()
    {
        if (statesStack.Peek() != PauseStates.GeneralPause)
        {
            statesStack.Push(PauseStates.GeneralPause);
        }
        
        PauseMenuUI.SetActive(true);
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
        SingletonInputManager.instance.InputMap.Disable();
        player.SetActive(false);
        Time.timeScale = 0f;
    }

    public void LoadMainMenu()
    {
        Time.timeScale = 1f;
        SceneManager.UnloadSceneAsync("SampleScene");
        SceneManager.LoadSceneAsync("Menu");
    }

    public void QuitGame()
    {
        Debug.Log("Exit");
        Application.Quit();
    }

    private void OnOpenInventory()
    {
        if (statesStack.Peek() != PauseStates.InventoryPause)
            statesStack.Push(PauseStates.InventoryPause);
    }
    
    private void OnCloseInventory()
    {
        if (statesStack.Peek() == PauseStates.InventoryPause)
            statesStack.Pop();
    }
}
