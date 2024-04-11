using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SettingsScript : MonoBehaviour
{
    public Slider Slider;
    public GameObject Player;

    void Start()
    {
        Slider.minValue = 0.1f;
        Slider.maxValue = 10f;
    }
    
    void Update()
    {
        var playerScript = Player.GetComponent<FirstPersonController>();
        playerScript.mouseSensitivity = Slider.value;
    }
}
