using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public interface IInventoryItem
{
    public void OnSelect();

    public void OnUnselect();

    public bool TryUsePrimaryAction();

    public bool TryUseSecondaryAction();
}
