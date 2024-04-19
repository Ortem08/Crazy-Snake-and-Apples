
using System;
using System.Collections.Generic;
using JetBrains.Annotations;

public class Inventory
{
    public List<IInventoryItem> Items { get; }

    public int Capacity { get; }

    [CanBeNull] public IInventoryItem SelectedItem => Items[SelectedIndex]; //null <=> nothing selected (or empty cell selected)

    private int _selectedIndex = 0;

    public int SelectedIndex {
        get { return _selectedIndex; }
        set
        {
            if (value < 0 || value >= Capacity)
            {
                throw new ArgumentOutOfRangeException(nameof(value));
            }
            _selectedIndex = value;
        } 
    }

    public Inventory(int capacity = 4)
    {
        if (capacity <= 0)
        {
            throw new ArgumentOutOfRangeException(nameof(capacity));
        }

        Capacity = capacity;
        Items = new(capacity);
        for (int i = 0; i < capacity; i++)
        {
            Items.Add(null);
        }
    }

    public void SelectNext()
    {
        SelectedIndex = (SelectedIndex + 1) % Capacity;
    }

    public void SelectPrevious()
    {
        SelectedIndex = (Capacity + SelectedIndex - 1) % Capacity;
    }
}