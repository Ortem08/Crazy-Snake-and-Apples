using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameSceneManager : MonoBehaviour
{
    private bool playerDied = false;

    private bool playerWin = false;

    [SerializeField]
    private GameObject key;

    [SerializeField]
    private string SceneToGoOnPlayerDead = "DeadSceneDemo";

/*    [SerializeField]
    private string SceneToGoOnPlayerWin;*/



    protected void Start()
    {
        if (key == null)
        {
            throw new System.Exception("key not set");
        }

        GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerComponent>().OnPlayerDead += OnPlayerDied;
        GameObject.FindGameObjectWithTag("Snake").GetComponent<SnakeController>().OnSnakeDefeat += OnSnakeDeath;
    }
/*
    private void OnDestroy()
    {
        GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerComponent>().OnPlayerDead -= OnPlayerDied;
        GameObject.FindGameObjectWithTag("Snake").GetComponent<SnakeController>().OnSnakeDefeat -= OnSnakeDeath;
    }*/

    private void OnPlayerDied()
    {
        if (playerDied) return;
        playerDied = true;

        SceneManager.LoadScene(SceneToGoOnPlayerDead);
    }

    private void OnSnakeDeath(Vector3 position)
    {
        key.transform.position = position;
    }

    // call this when opened door and activated trigger
    public void OnPlayerWin()
    {
        if (playerWin) return;
        playerWin = true;

        GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerComponent>().Resurrect();
        PlayerWinAction();
    }

    protected virtual void PlayerWinAction()
    {
        //SceneManager.LoadScene(SceneToGoOnPlayerWin);
    }
}
