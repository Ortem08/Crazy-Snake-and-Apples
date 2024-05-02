using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

public class ProjectileFactory : MonoBehaviour
{
    private Instantiator instantiator;

    [SerializeField]
    private List<GameObject> projectilePrefabs;    // no new() here bc unity will replace it

    private readonly Dictionary<string, GameObject> spellToPrefabMap = new();

    private void Awake()
    {
        if (!TryGetComponent<Instantiator>(out instantiator))
        {
            throw new System.Exception("Projectile factory needs instantiator");
        }
    }

    private void Start()
    {
        foreach (var prefab in projectilePrefabs)
        {
            if (prefab.TryGetComponent<IProjectile>(out var proj))
            {
                var spell = proj.GetType().Name.ToLower();
                spellToPrefabMap.Add(spell, prefab);
            }
            else
            {
                throw new System.Exception($"prefab {prefab} is not a projectile");
            }
        }
    }

    public IProjectileTreeNode AssembleProjectileTree(List<string> spells)
    {
        ProjectileTreeNode parentNode = null;
        ProjectileTreeNode root = null;
        foreach (var spell in spells)
        {
            var projectilePrefab = ResolveProjectileSpellToPrefab(spell);

            if (projectilePrefab != null)
            {
                var nextNode = new ProjectileTreeNode(projectilePrefab, instantiator, parentNode);
                if (parentNode != null)
                {
                    parentNode.OnProjectileEvent += (info) =>
                    {
                        if (info.TryGetProjectileInfo<IHitSomethingInfo>(out var hitSomethingInfo))
                        {
                            var instance = nextNode.InstantiateProjectile();
                            var projectile = instance.GetComponent<IProjectile>();

                            var d = hitSomethingInfo.ProjectileDirection.normalized;
                            var n = hitSomethingInfo.SurfaceNormal.normalized;

                            var reflectionDirection = d - 2 * Vector3.Dot(d, n) * n;    // applied algebra
                            projectile.Fire(hitSomethingInfo.Position, reflectionDirection.normalized);
                        }
                        else if (info.TryGetProjectileInfo<IExpirationInfo>(out var expirationInfo))
                        {
                            var instance = nextNode.InstantiateProjectile();
                            var projectile = instance.GetComponent<IProjectile>();
                            projectile.Fire(expirationInfo.Position, expirationInfo.ProjectileDirection);
                        }
                    };
                }

                parentNode = nextNode;

                if (root == null)
                {
                    root = parentNode;
                }

                continue;
            }

            var modifier = ResolveModifierSpellToModifier(spell);
            if (modifier != null)
            {
                parentNode.AddModifiers(new[] { modifier });      //kinda bad
                continue;
            }

            throw new System.Exception($"spell {spell} not found");
        }
        return root;
    }

    private GameObject ResolveProjectileSpellToPrefab(string spell)
    {
        if (spellToPrefabMap.TryGetValue(spell.ToLower(), out var prefab))
        {
            return prefab;
        }
        return null;
    }

    private IModifier ResolveModifierSpellToModifier(string spell)
    {
        //nothing changes
        spell = spell.ToLower();
        if (spell == typeof(PiercingModifier).Name.ToLower())
        {
            return new PiercingModifier();
        }
        if (spell == typeof(ConstantDamageIncreaseModifier).Name.ToLower())
        {
            return new ConstantDamageIncreaseModifier();
        }
        if (spell == typeof(BouncyModifier).Name.ToLower())
        {
            return new BouncyModifier();
        }

        return null;
    }
}
