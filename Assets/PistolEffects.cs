using UnityEngine;

public class PistolEffects : MonoBehaviour, IWeaponEffects
{
    //public Animator animator;
    //public AudioSource audioSource;
    //public Camera playerCamera;

    PistolEffects()
    {
        //audioSource = GetComponent<AudioSource>();
    }

    public void PlayShootAnimation()
    {
        //animator.SetTrigger("Shoot");
    }

    public void PlaySoundEffect()
    {
        //audioSource.Play();
    }

    public void ApplyRecoil()
    {
        // Логика отдачи
    }

    public void CameraShake()
    {
        // Логика тряски камеры
    }
}