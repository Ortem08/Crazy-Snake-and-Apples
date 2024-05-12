using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.SceneManagement;
using UnityEngine.Serialization;
using Random = System.Random;

public class SoundController : MonoBehaviour
{
    public GameObject SoundObject;
    private AudioMixer AudioMixer { get; set; }

    void Start()
    {
        AudioMixer = Resources.Load<AudioMixer>("AudioMixer");
        var generalVolume = PlayerPrefs.HasKey("Volume") ? PlayerPrefs.GetFloat("Volume") : 1f;
        AudioMixer.SetFloat("MasterVolume", 80 * (generalVolume - 1));
        
        switch (SceneManager.GetActiveScene().name)
        {
            case "Menu":
                PlayBackground("MenuTheme", 0.5f);
                break;
            case "1105Merge": //Level 1
                PlayBackground("LevelTheme1", 0f);
                break;
            case "Level 2": //Level 2
                PlayBackground("LevelTheme2", 0f);
                break;
        }
    }

    // ReSharper disable Unity.PerformanceAnalysis
    public float PlaySound(string soundType, float volume, Vector3 position, GameObject parent=null)
    {
        var length = 0f;
        try
        {
            var filesCount = Directory.GetFiles($"Assets/Resources/Sounds/{soundType}").Length / 2;
            var random = new Random();

            GameObject soundObject;
            if (parent != null)
            {
                soundObject = Instantiate(SoundObject, parent!.transform, true);
                soundObject.transform.position = parent!.transform.position;
                soundObject.transform.rotation = parent.transform.rotation;
            }
            else
            {
                soundObject = Instantiate(SoundObject, position, Quaternion.identity);
            }

            soundObject.name = soundType;

            var audioSource = soundObject.AddComponent<AudioSource>();
            var path = $"Sounds/{soundType}/{soundType}_{random.Next(1, filesCount)}";

            audioSource.clip = Resources.Load<AudioClip>(path);
            audioSource.outputAudioMixerGroup = AudioMixer.FindMatchingGroups("Master")[0];
            audioSource.volume = volume;
            audioSource.spatialBlend = 1f;
            audioSource.Play();

            length = audioSource.clip.length;
            Destroy(soundObject, audioSource.clip.length);
        }
        catch (System.Exception exc)
        {
            Debug.LogException(exc);
        }
        
        return length;
    }

    public void PlayBackground(string name, float volume)
    {
        Debug.Log(SceneManager.GetActiveScene().name);
        
        var soundObject = Instantiate(SoundObject);
        
        soundObject.name = name;
        
        var audioSource = soundObject.AddComponent<AudioSource>();
        var path = $"Sounds/Background/{name}";

        audioSource.clip = Resources.Load<AudioClip>(path);
        
        audioSource.outputAudioMixerGroup = AudioMixer.FindMatchingGroups("Master")[0];
        audioSource.volume = volume;
        audioSource.spatialBlend = 0f;
        audioSource.loop = true;
        audioSource.Play();
    }
}
