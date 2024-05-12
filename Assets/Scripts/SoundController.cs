using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.Serialization;
using Random = System.Random;

public class SoundController : MonoBehaviour
{
    public float GeneralVolume;
    public GameObject SoundObject;
    
    public float PlaySound(string soundType, float volume, Vector3 position, GameObject parent=null)
    {
        UpdateFields();
        
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
            audioSource.volume = GeneralVolume * volume;
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

    private void UpdateFields()
    {
        GeneralVolume = PlayerPrefs.HasKey("Volume") ? PlayerPrefs.GetFloat("Volume") : 1;
    }
}
