using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public struct DamageInfo
{
    public float Amount;

    public DamageType Type;

    // direction

    // any other thing

    public DamageInfo(float amount = 0, DamageType type = DamageType.ProjectileDamage)
    {
        Amount = amount;        // han we heal by damage?
        Type = type;
    }

    public DamageInfo SetAmount(float amount)
    {
        return new DamageInfo(amount, Type);
    }

    public DamageInfo SetType(DamageType type)
    {
        return new DamageInfo(Amount, type);
    }
}
