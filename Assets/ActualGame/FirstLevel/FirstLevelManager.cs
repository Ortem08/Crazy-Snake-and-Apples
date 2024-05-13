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
        
        var sources = FindObjectsOfType<AudioSource>();
        foreach (var source in sources)
        {
            if (source.name == "LevelTheme1")
            {
                Destroy(source);
                break;
            }
        }
            
        GameObject.FindGameObjectWithTag("SoundController")
            .GetComponent<SoundController>()
            .PlayBackground("LevelTheme2", 0.3f);
    }

    protected override void SnakeDefeatAction()
    {
        notificationManager?.Notify("”шел, гад ползучий! [ ---===e ]");
    }
}
