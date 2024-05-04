using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;

public class LaggyPistol : MonoBehaviour, IInventoryItem
{
    [SerializeField]
    private GameObject onSceneAvatar;

    [SerializeField]
    private GameObject inHandAvatar;

    [SerializeField]
    private Transform tipOfTheGun;

    // temporary (before GUI is in use)
    [SerializeField]
    private List<string> spells;

    private ProjectileFactory projectileFactory;

    private IUser user;

    private Collider colliderForDetection;

    private SoundController soundController;

    private Animator animator;

    private float cooldown = 1f / 3f;
    private float lastShotTime;

    private void Awake()
    {
        colliderForDetection = GetComponent<Collider>();
    }

    private void Start()
    {
        animator = inHandAvatar.GetComponent<Animator>();

        var soundControllerObject = GameObject.FindGameObjectWithTag("SoundController");
        if (soundControllerObject == null || !soundControllerObject.TryGetComponent<SoundController>(out soundController))
        {
            Debug.LogWarning("Sound Controller not found - sounds may not work");
        }

        var projectileFactoryGameObject = GameObject.FindGameObjectWithTag("ProjectileFactory");
        if (projectileFactoryGameObject == null || !projectileFactoryGameObject.TryGetComponent(out projectileFactory))
        {
            throw new System.Exception("weapon needs a projectile factory on scene; (use prefab)");
        }
    }

    public void DropOut()
    {
        transform.parent = null;
        transform.eulerAngles = Vector3.zero;
        onSceneAvatar.SetActive(true);
        colliderForDetection.enabled = true;
    }

    public void OnSelect()
    {
        inHandAvatar.SetActive(true);
    }

    public void OnUnselect()
    {
        inHandAvatar.SetActive(false);
    }

    public void SetUser(IUser user)
    {
        colliderForDetection.enabled = false;
        this.user = user;
        onSceneAvatar.SetActive(false);

        transform.parent = user.CameraTransform;
        transform.localPosition = Vector3.zero + new Vector3(0.2f, -0.2f, 1f);

        inHandAvatar.transform.forward = user.CameraTransform.forward;
        inHandAvatar.transform.Rotate(new Vector3(270, 0, 0));
    }

    public bool TryUsePrimaryAction()
    {
        if (Time.time - lastShotTime < cooldown)
            return false;

        if (!animator.GetBool("canShoot"))
            return false;

        

        var tree = projectileFactory.AssembleProjectileTree(spells);
        if (tree == null)
        {
            Debug.Log("insert spell please");
            return false;
        }

        var instance = tree.InstantiateProjectile();

        if (instance == null)
        {
            Debug.Log("insert spell please");
            return false;
        }

        if (instance.TryGetComponent<IProjectile>(out var projectile))
        {
            Vector3 shootDirection = user.CameraTransform.forward;
            var delta = (user.CameraTransform.right - user.CameraTransform.up) * 0.02f;
            Vector3 startPosition = user.CameraTransform.position + shootDirection * 0.1f;

            if (projectile is GunShot)
            {
                (projectile as GunShot).SetVisibleRayBeginning(tipOfTheGun.position);
            }

            projectile.Fire(startPosition, shootDirection, user.Velocity);

            if (soundController != null)
            {
                soundController.PlaySound("PistolShot", startPosition + user.CameraTransform.forward, 0.8f);
            }
        }
        else
        {
            throw new System.Exception("WTF?! Instance is not a projectile. Thats forbidden by law!");
        }

        lastShotTime = Time.time;
        animator.SetTrigger("IsShooting");
        
        return true;
    }

    public bool TryUseSecondaryAction()
    {
        Debug.Log("Shoot yourself if the leg. Now.");
        return true;
    }
}
