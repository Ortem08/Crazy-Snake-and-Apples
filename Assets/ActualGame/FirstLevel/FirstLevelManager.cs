using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FirstLevelManager : GameSceneManager
{
    [SerializeField]
    private GameObject SecondLevel;

    [SerializeField]
    private NotificationManager notificationManager;

    protected override void PlayerWinAction()
    {
        Debug.Log(SecondLevel);
        gameObject.SetActive(false);
        SecondLevel.SetActive(true);
    }

    protected override void SnakeDefeatAction()
    {
        notificationManager?.Notify("”шел, гад ползучий! [ ---===e ]");
    }
}
