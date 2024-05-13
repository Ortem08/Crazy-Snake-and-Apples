using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class SettingsScript : MonoBehaviour
{
    public Slider SensetivitySlider;
    public Slider VolumeSlider;

    private AudioMixer AudioMixer;
    
    void Start()
    {
        AudioMixer = Resources.Load<AudioMixer>("AudioMixer");
        
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
        AudioMixer.SetFloat("MasterVolume", 80 * (VolumeSlider.value - 1));
    }

    void LoadSliderValue()
    {
        if (PlayerPrefs.HasKey("Sensetivity"))
            SensetivitySlider.value = PlayerPrefs.GetFloat("Sensetivity");
        else
            SensetivitySlider.value = 10f;

        if (PlayerPrefs.HasKey("Volume"))
            VolumeSlider.value = PlayerPrefs.GetFloat("Volume");
        else
            VolumeSlider.value = 0.7f;
    }
}
