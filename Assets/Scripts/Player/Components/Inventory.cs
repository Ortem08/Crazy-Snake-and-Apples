
using System.Collections.Generic;

public class Inventory
{
    public List<IInventoriable> items { get; } = new();

    public void Add(IInventoriable item) => items.Add(item);

    public void Remove(IInventoriable item) => items.Remove(item);
}