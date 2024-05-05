using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface ICardBasedItem : IInventoryItem
{
    public CardInventory CardInventory { get; }
}
