using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class SettingsScript : MonoBehaviour
{
    public Slider SensetivitySlider;
    public Slider VolumeSlider;

    void Start()
    {
        SensetivitySlider.minValue = 0.1f;
        SensetivitySlider.maxValue = 10f;
        
        VolumeSlider.minValue = 0f;
        VolumeSlider.maxValue = 1f;
        LoadSliderValue();
    }
    
    void Update()
    {
        PlayerPrefs.SetFloat("Sensetivity", SensetivitySlider.value);
        
        PlayerPrefs.SetFloat("Volume", VolumeSlider.value);
    }

    void LoadSliderValue()
    {
        if (PlayerPrefs.HasKey("Sensetivity"))
            SensetivitySlider.value = PlayerPrefs.GetFloat("Sensetivity");
        
        if (PlayerPrefs.HasKey("Volume"))
            VolumeSlider.value = PlayerPrefs.GetFloat("Volume");
    }
}
