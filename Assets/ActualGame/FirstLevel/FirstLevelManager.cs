using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FirstLevelManager : GameSceneManager
{
    [SerializeField]
    private GameObject SecondLevel;

    protected override void PlayerWinAction()
    {
        Debug.Log(SecondLevel);
        gameObject.SetActive(false);
        SecondLevel.SetActive(true);
    }
}
