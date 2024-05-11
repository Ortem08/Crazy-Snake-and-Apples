using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class DeadSceneManager : MonoBehaviour
{
    [SerializeField]
    private RestartTrigger trigger;

    private bool gameStarted = false;

    private void Start()
    {
        trigger.OnTriggerEvent += StartGame;
    }

    private void StartGame()
    {
        if (gameStarted) return;
        SceneManager.LoadScene("GameSceneDemo");
    }
}
