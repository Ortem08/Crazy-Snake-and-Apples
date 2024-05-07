using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PiercingModifier : IModifier
{
    public bool TryModify(IModifiable modifiable)
    {
        if (modifiable.TryGetModificationInterface<IPierceable>(out var pierceable))
        {
            pierceable.CanPierce = true;
            return true;
        }
        return false;
    }
}