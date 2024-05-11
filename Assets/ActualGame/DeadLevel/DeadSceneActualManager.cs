using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class DeadSceneActualManager : MonoBehaviour
{
    [SerializeField]
    private RestartTrigger trigger;


    [SerializeField]
    private string nextScene;

    private void Start()
    {
        trigger.OnTriggerEvent += Trigger_OnTriggerEvent;
    }

    private void Trigger_OnTriggerEvent()
    {
        SceneManager.LoadScene(nextScene);
    }
}
