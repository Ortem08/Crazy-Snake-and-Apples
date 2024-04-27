using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IProjectileTreeNode
{
    public IProjectileTreeNode Parent { get; }

    public IEnumerable<IProjectileTreeNode> Children { get; }

    public IEnumerable<IModifier> Modifiers { get; }

    public GameObject InstantiateProjectile();

    public GameObject InstantiateProjectile(Transform parent);

    public GameObject InstantiateProjectile(Vector3 position);
}
