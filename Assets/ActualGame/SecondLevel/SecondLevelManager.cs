using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SecondLevelManager : GameSceneManager
{
    [SerializeField]
    private string NextSceneName;

    protected override void PlayerWinAction()
    {
        if (NextSceneName != null && NextSceneName != "")
        {
            SceneManager.LoadScene(NextSceneName);
        }
    }
}
