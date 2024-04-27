using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ProjectileTreeNode : IProjectileTreeNode
{
    public IProjectileTreeNode Parent { get; }

    public IEnumerable<IProjectileTreeNode> Children { get; } = new List<IProjectileTreeNode>();

    public IEnumerable<IModifier> Modifiers { get; } = new List<IModifier>();

    private readonly Instantiator instantiator;

    private readonly GameObject projectilePrefab;

    public ProjectileTreeNode(GameObject projectilePrefab, Instantiator instantiator,
        IProjectileTreeNode parent = null)
    {
        this.projectilePrefab = projectilePrefab;
        this.instantiator = instantiator;
        Parent = parent;
    }

    public void AddModifiers(IEnumerable<IModifier> modifiers)
    {
        foreach (var mod in modifiers)
        {
            Modifiers.Append(mod);
        }
    }

    public GameObject InstantiateProjectile()
    {
        var resultGameobject = instantiator.InstantiatePrefab(projectilePrefab);
        if (resultGameobject.TryGetComponent<IModifiable>(out var resultModifiable))
        {
            // this part might be changed if we want modifiers to "modify" modifiers
            foreach (var modifier in Modifiers)
            {
                modifier.TryModify(resultModifiable);
            }
        }
        
        return resultGameobject;
    }

    public GameObject InstantiateProjectile(Transform parent)
    {
        var result = InstantiateProjectile();
        result.transform.parent = parent;
        return result;
    }

    public GameObject InstantiateProjectile(Vector3 position)
    {
        var result = InstantiateProjectile();
        result.transform.position = position;
        return result;
    }
}
