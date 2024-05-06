using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CanonBall : ProjectileBase, IDamaging
{
    public DamageInfo DamageInfo { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

    public override event Action<IProjectileInfo> OnProjectileEvent;

    public override void Fire(Vector3 origin, Vector3 direction, Vector3 baseVelocity = default)
    {
        throw new NotImplementedException();
    }
}
