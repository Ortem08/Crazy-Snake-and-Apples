using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.Audio;
using Random = System.Random;

public class SoundController : MonoBehaviour
{
    private AudioMixer mixer;
    
    private static Dictionary<string, string> nameToPath;
    private void Start()
    {
        
        nameToPath = new Dictionary<string, string>()
        {
            ["PistolShot"] = "Sounds/PistolShot"
        };
    }

    public void PlaySound(string soundType, Vector3 position, float volume)
    {
        var filesCount = Directory.GetFiles($"Assets/Resources/{nameToPath[soundType]}").Length / 2;
        var random = new Random();
        
        var soundObject = new GameObject(soundType);
        soundObject.transform.position = position;

        var audioSource = soundObject.AddComponent<AudioSource>();
        var path = $"{nameToPath[soundType]}/{soundType}_{random.Next(1, filesCount)}";
        
        audioSource.clip = Resources.Load<AudioClip>(path);
        audioSource.volume = volume;
        audioSource.spatialBlend = 1f;
        audioSource.Play();
        Destroy(soundObject, audioSource.clip.length);
    }
    
}
