using System.Collections;
using System.Collections.Generic;
using UnityEditor.Build;
using UnityEngine;

public class MagicWeapon : MonoBehaviour, IInventoryItem
{
    private IUser user;

    private Collider colliderForDetection;

    private ItemAvatar avatar;

    public List<string> Spells;

    public GameObject GunShotPrefab;

    public GameObject ExplosionPrefab;

    public Instantiator Instantiator;     // this one needed to create instances of spells

    public void Awake()
    {
        colliderForDetection = GetComponent<Collider>();
        avatar = GetComponent<ItemAvatar>();
    }

    public void DropOut()
    {
        transform.parent = null;
        colliderForDetection.enabled = true;
        gameObject.SetActive(true); //to lazy to lay my hands on actual model.
                                    //Probably should have used MeshRenderer component
        avatar.enabled = true;
    }

    public void OnSelect()
    {
        gameObject.SetActive(true);
    }

    public void OnUnselect()
    {
        gameObject.SetActive(false);
    }

    public void SetUser(IUser user)
    {
        avatar.enabled=false;
        colliderForDetection.enabled=false;
        this.user = user;
        gameObject.SetActive(true);
        transform.parent = user.HandTransform;
        transform.localPosition = Vector3.zero;
        transform.localEulerAngles = Vector3.zero;
    }

    public bool TryUsePrimaryAction()
    {
        var instance = AssembleSpellTree()?.InstantiateProjectile();

        if (instance == null)
        {
            Debug.Log("insert spell please");
            return false;
        }

        if (instance.TryGetComponent<IProjectile>(out var projectile))
        {
            Vector3 shootDirection = user.CameraTransform.forward;
            Vector3 startPosition = user.CameraTransform.position + shootDirection * 0.1f;
            projectile.Fire(startPosition, shootDirection);
        }
        else
        {
            throw new System.Exception("WTF?! Instance is not a projectile. Thats forbidden by law!");
        }
        return true;
    }

    private IProjectileTreeNode AssembleSpellTree()
    {
        ProjectileTreeNode currentNode = null;
        ProjectileTreeNode root = null;
        foreach (var spell in Spells)
        {
            ProjectileTreeNode nextNode = null;
            if (spell.ToLower() == "gunshot")
            {
                nextNode = new ProjectileTreeNode(GunShotPrefab, Instantiator, currentNode);
            }
            else if (spell.ToLower() == "explosion")
            {
                nextNode = new ProjectileTreeNode(ExplosionPrefab, Instantiator, currentNode);
            }
            
            if (nextNode == null)
            {
                throw new System.Exception($"Could not find {spell}");
            }

            if (currentNode != null)
            {
                currentNode.OnProjectileEvent += (info) =>
                {
                    if (info.TryGetProjectileInfo<ILocationInfo>(out var locationInfo))
                    {
                        var instance = nextNode.InstantiateProjectile();
                        var projectile = instance.GetComponent<IProjectile>();
                        projectile.Fire(locationInfo.Position, locationInfo.Direction);
                    }
                };
            }
            currentNode = nextNode;

            if (root == null)
            {
                root = currentNode;
            }
        }
        return root;
    }

    public bool TryUseSecondaryAction()
    {
        Debug.Log("action never took place. Or did it?");
        return true;
    }
}
