using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;

public class Canon : MonoBehaviour, ICardBasedItem, IChargeable
{
    [SerializeField]
    private List<Spell> spells;

    [SerializeField]
    private bool useInspectorSpellList;

    public CardInventory CardInventory { get; } = new CardInventory(10);

    public ChargeInfo ChargeInfo { get; private set; } = new ChargeInfo(1);

    public event Action<ChargeInfo> OnChargeChanged;

    private Collider canonCollider;

    private ProjectileFactory projectileFactory;

    private IUser user;

    [SerializeField]
    private Texture2D spriteTexture;

    private Sprite sprite;

    [SerializeField]
    private GameObject avatar;

    private void Awake()
    {
        canonCollider = GetComponent<Collider>();
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

        canonCollider.enabled = true;

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
        EnsureInHandPosition();
    }

    public void OnUnselect()
    {
        avatar.SetActive(false);
    }

    public void SetUser(IUser user)
    {
        canonCollider.enabled = false;

        this.user = user;
        transform.parent = user.CameraTransform;

        avatar.SetActive(true);
        EnsureInHandPosition();
    }

    public bool TryUsePrimaryAction()
    {
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
                Vector3 startPosition = user.CameraTransform.position + shootDirection * 0.1f;

                if (projectile.TryGetModificationInterface<IUserSecure>(out var userSecure))
                {
                    userSecure.EnsureProtectionOfObjectWith(user.UserGameObject.GetInstanceID());
                }

                projectile.Fire(startPosition, shootDirection, user.Velocity);
            }
            else
            {
                throw new Exception("WTF?! Instance is not a projectile. Thats forbidden by law!");
            }
        }

        return true;
    }

    public bool TryUseSecondaryAction()
    {
        var length = 3;
        var width = 2;
        var height = 2;

        var boxCenter = user.CameraTransform.position + user.CameraTransform.forward * (length / 2);
        var halfExdends = new Vector3(width / 2, height / 2, length / 2);

        foreach (var collider in Physics.OverlapBox(boxCenter, halfExdends, user.CameraTransform.rotation, LayersStorage.PossiblyHurtables))
        {
            if (user.UserGameObject == collider.gameObject)
            {
                continue;
            }
            if (collider.gameObject.TryGetComponent<IHurtable>(out var hurtable))
            {
                hurtable.TakeDamage(new DamageInfo(1, DamageType.MeleeDamage));
            }
            if (collider.gameObject == null)
            {
                continue;
            }
            if (collider.gameObject.TryGetComponent<IPushable>(out var pushable))
            {
                pushable.Push(user.CameraTransform.forward * 10);
            }
            else if (collider.gameObject.TryGetComponent<Rigidbody>(out var rb))
            {
                rb.AddForce(user.CameraTransform.forward * 10, ForceMode.Impulse);
            }
        }

        return true;
    }

    private void EnsureInHandPosition()
    {
        transform.forward = user.CameraTransform.forward;
        transform.localPosition = new Vector3(0.6f, -0.6f, 1f);
    }
}
