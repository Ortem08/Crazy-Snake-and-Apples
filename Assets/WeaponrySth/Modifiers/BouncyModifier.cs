using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BouncyModifier : IModifier
{
    public bool TryModify(IModifiable modifiable)
    {
        if (modifiable.TryGetModificationInterface<IBouncing>(out var bouncing))
        {
            bouncing.BounceLevel += 1;
            return true;
        }
        return false;
    }
}
