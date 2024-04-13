
using System.Collections.Generic;

public class Inventory
{
    private List<IInventoriable> items;

    public void Add(IInventoriable item) => items.Add(item);

    public void Remove(IInventoriable item) => items.Remove(item);
}