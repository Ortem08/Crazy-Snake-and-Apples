using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExitScript : MonoBehaviour, IInventoryItem
{
    public void DropOut()
    {
        throw new System.NotImplementedException();
    }

    public Sprite GetItemAvatarSprite()
    {
        throw new System.NotImplementedException();
    }

    public void OnSelect()
    {
        
    }

    public void OnUnselect()
    {
        throw new System.NotImplementedException();
    }

    public void SetUser(IUser user)
    {
        Debug.Log("Exit");
        Application.Quit();
    }

    public bool TryUsePrimaryAction()
    {
        throw new System.NotImplementedException();
    }

    public bool TryUseSecondaryAction()
    {
        throw new System.NotImplementedException();
    }
}
