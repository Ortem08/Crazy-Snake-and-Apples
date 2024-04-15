using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerParamsUpdater : MonoBehaviour
{
    public GameObject Player;

    private FirstPersonController controller;
    void Start()
    {
        controller = Player.GetComponent<FirstPersonController>();

        SetSensetivity();
    }
    
    void Update()
    {
        SetSensetivity();
    }

    void SetSensetivity()
    {
        if (PlayerPrefs.HasKey("Sensetivity"))
            controller.mouseSensitivity = PlayerPrefs.GetFloat("Sensetivity");
    }
}
