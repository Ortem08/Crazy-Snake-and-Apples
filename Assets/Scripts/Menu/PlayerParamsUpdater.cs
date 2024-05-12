using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerParamsUpdater : MonoBehaviour
{
    private GameObject player;

    private const int MouseSensetivityCoef = 28;
    
    private QuakeCPMPlayerMovement controller;
    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");
        controller = player.GetComponent<QuakeCPMPlayerMovement>();

        SetSensetivity();
    }
    
    void Update()
    {
        SetSensetivity();
    }

    void SetSensetivity()
    {
        if (PlayerPrefs.HasKey("Sensetivity"))
        {
            controller.xMouseSensitivity = PlayerPrefs.GetFloat("Sensetivity") * MouseSensetivityCoef;
            controller.yMouseSensitivity = controller.xMouseSensitivity;
        }
    }
}
