using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.Audio;
using Random = System.Random;

public class SoundController : MonoBehaviour
{
    public GameObject SoundObject;
    
    private AudioMixer mixer;
    
    private static Dictionary<string, string> nameToPath;

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
    
}
