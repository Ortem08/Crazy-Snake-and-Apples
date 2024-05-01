using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public interface IInventoryItem
{
    public GameObject GetItemAvatarSkin() => null;
    
    public void OnSelect();

    public void OnUnselect();

    public bool TryUsePrimaryAction();

    public bool TryUseSecondaryAction();

    public void SetUser(IUser user);

    public void DropOut();  // means that item should drop
}
