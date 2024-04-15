
using System.Collections.Generic;
using JetBrains.Annotations;

public class Inventory
{
    public List<IITem> Items { get; } = new();
    [CanBeNull] public IITem SelectedItem { get; set; }
}