using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class CheapPistol : MonoBehaviour, ICardBasedItem, IChargeable
{
    public CardInventory CardInventory { get; private set; }

    public ChargeInfo ChargeInfo { get; private set; }

    public event Action<ChargeInfo> OnChargeChanged;

    private ProjectileFactory projectileFactory;

    [SerializeField]
    private GameObject avatar;

    [SerializeField]
    private Transform tipOfTheGun;

    [SerializeField]
    private Texture2D spriteTexture;

    private Sprite sprite;

    private Collider colliderForDetection;

    private IUser user;

    // spells

    // temporary (before GUI is in use)
    [SerializeField]
    private List<Spell> spells;

    // end spells

    [SerializeField]
    private bool useInspectorSpellList = true;

    private void Awake()
    {
        CardInventory = new CardInventory(7);   // cheap
        ChargeInfo = new ChargeInfo(30); // im tired of reloading
        colliderForDetection = GetComponent<Collider>();
    }

    private void Start()
    {
        var projectileFactoryGameObject = GameObject.FindGameObjectWithTag("ProjectileFactory");
        if (projectileFactoryGameObject == null || !projectileFactoryGameObject.TryGetComponent(out projectileFactory))
        {
            throw new System.Exception("weapon needs a projectile factory on scene; (use prefab)");
        }

        if (avatar == null)
        {
            throw new Exception($"avatar for {name} not set");
        }

        if (tipOfTheGun == null)
        {
            throw new Exception($"tip of the gun transform for {name} not set");
        }

        if (spriteTexture == null)
        {
            throw new Exception("sprite texture not found");
        }

        sprite = Sprite.Create(spriteTexture,
            new Rect(0.0f, 0.0f, spriteTexture.width, spriteTexture.height),
                new Vector2(0.5f, 0.5f)
            );
    }

    public void DropOut()
    {
        transform.parent = null;
        transform.rotation = Quaternion.identity;

        avatar.SetActive(true);
        avatar.transform.localPosition = Vector3.zero;
        avatar.transform.localRotation = Quaternion.identity;

        colliderForDetection.enabled = true;

        OnChargeChanged = null;

        user = null;
    }

    public Sprite GetItemAvatarSprite()
    {
        return sprite;
    }

    public void OnSelect()
    {
        avatar.SetActive(true);
    }

    public void OnUnselect()
    {
        avatar.SetActive(false);
    }

    public void SetUser(IUser user)
    {
        colliderForDetection.enabled = false;

        this.user = user;
        transform.parent = user.CameraTransform;

        transform.localPosition = Vector3.zero + new Vector3(0.4f, -0.2f, 1f);

        transform.forward = user.CameraTransform.forward;

        avatar.transform.localEulerAngles = new Vector3(90, 0, 0);
    }

    public bool TryUsePrimaryAction()
    {
        transform.forward = user.CameraTransform.forward;

        var spellList = spells;

        if (!useInspectorSpellList)
        {
            spellList = CardInventory.Cards.Where(card => card != null).Select(card => card.Spell).ToList(); // crime agains humanity
        }

        var projectileForest = projectileFactory.AssembleProjectileForest(spellList);

        if (projectileForest.Count == 0)
        {
            Debug.Log("insert spell please");
            return false;
        }

        foreach (var tree in projectileForest)
        {
            var instance = tree.InstantiateProjectile();
            if (instance.TryGetComponent<IProjectile>(out var projectile))
            {
                Vector3 shootDirection = user.CameraTransform.forward;
                var delta = (user.CameraTransform.right - user.CameraTransform.up) * 0.02f;
                Vector3 startPosition = user.CameraTransform.position + shootDirection * 0.1f;

                if (projectile is GunShot)
                {
                    (projectile as GunShot).SetVisibleRayBeginning(tipOfTheGun.position);
                }

                if (projectile.TryGetModificationInterface<IUserSecure>(out var userSecure))
                {
                    userSecure.EnsureProtectionOfObjectWith(user.UserGameObject.GetInstanceID());
                }

                projectile.Fire(startPosition, shootDirection, user.Velocity);
            }
            else
            {
                throw new System.Exception("WTF?! Instance is not a projectile. Thats forbidden by law!");
            }
        }

        return true;
    }

    public bool TryUseSecondaryAction()
    {
        Debug.Log("Nope");
        return true;
    }
}
