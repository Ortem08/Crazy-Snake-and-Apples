using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameSceneManager : MonoBehaviour
{
    private bool playerDied = false;


    void Start()
    {
        GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerComponent>().OnPlayerDead += OnPlayerDied;
    }

    private void OnPlayerDied()
    {
        if (playerDied) return;
        playerDied = true;

        SceneManager.LoadScene("DeadSceneDemo");
    }
}
