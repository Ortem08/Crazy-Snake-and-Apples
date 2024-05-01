
using System;
using System.Collections.Generic;
using System.Net.Mime;
using JetBrains.Annotations;
using static UnityEditor.Progress;
using UnityEditor.UI;

public class Inventory
{
    public List<IInventoryItem> Items { get; }
    // public List<Image> Items { get; }

    public int Capacity { get; }

    [CanBeNull] public IInventoryItem SelectedItem { 
        get => Items[SelectedIndex];
        set
        {
            Items[_selectedIndex] = value;
            SetItemUI(); 
        } // might be better to check if no item already here
    }

    private int _selectedIndex = 0;

    public int SelectedIndex {
        get { return _selectedIndex; }
        set
        {
            if (value < 0 || value >= Capacity)
            {
                throw new ArgumentOutOfRangeException(nameof(value));
            }
            SelectedItem?.OnUnselect();
            _selectedIndex = value;
            SelectedItem?.OnSelect();
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

    public bool TryDropCurrent()
    {
        if (SelectedItem == null)
        {
            return false;
        }

        SelectedItem.OnUnselect();
        SelectedItem.DropOut();
        SelectedItem = null;

        return true;
    }

    private void SetItemUI()
    {
    }
    
    private void UnsetItemUI()
    {
        
    }
}