using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameSceneManager : MonoBehaviour
{
    private bool playerDied = false;

    [SerializeField]
    private GameObject key;


    void Start()
    {
        if (key == null)
        {
            throw new System.Exception("key not set");
        }

        GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerComponent>().OnPlayerDead += OnPlayerDied;
        GameObject.FindGameObjectWithTag("Snake").GetComponent<SnakeController>().OnSnakeDefeat += OnSnakeDeath;
    }

    private void OnPlayerDied()
    {
        if (playerDied) return;
        playerDied = true;

        SceneManager.LoadScene("DeadSceneDemo");
    }

    private void OnSnakeDeath(Vector3 position)
    {
        key.transform.position = position;
    }
}
