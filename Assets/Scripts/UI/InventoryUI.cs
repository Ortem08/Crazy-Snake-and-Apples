using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InventoryUI : MonoBehaviour
{
    [SerializeField]
    private PlayerComponent player;
    
    [SerializeField] 
    private GameObject FastAccessInventory;
    [SerializeField] 
    private GameObject Inventory;

    // Первые fastAccessCapacity из totalCapacity под fastAccessInventory
    [SerializeField] 
    private Image[] FastAccessItemHolderImages;
    private GameObject[] ItemHolders;
    private Image[] ItemImages;
    
    [SerializeField]
    private GameObject itemHolderPrefab;
    
    [SerializeField]
    private int fastAccessCapacity = 4;
    
    private int totalCapacity;
    
    void Start()
    {
        LoadInventory();
        player.Inventory.OnCnangeIndex.AddListener(OnCnangeIndex);
        player.Inventory.OnPickUpItem.AddListener(OnPickUpItem);
        player.Inventory.OnDropItem.AddListener(OnDropItem);
    }

    private void LoadInventory()
    {
        totalCapacity = player.Inventory.Capacity;
        ItemHolders = new GameObject[totalCapacity];
        ItemImages = new Image[totalCapacity];
        FastAccessItemHolderImages = new Image[fastAccessCapacity];
        
        for (var i = 0; i < fastAccessCapacity; i++)
        {
            ItemHolders[i] = Instantiate(itemHolderPrefab, FastAccessInventory.transform);
            FastAccessItemHolderImages[i] = ItemHolders[i].GetComponentsInChildren<Image>()[1];
            ItemImages[i] = ItemHolders[i].GetComponent<Image>();
        }
        
        for (var i = fastAccessCapacity; i < totalCapacity - fastAccessCapacity; i++)
        {
            ItemHolders[i] = Instantiate(itemHolderPrefab, Inventory.transform);
            ItemImages[i] = ItemHolders[i].GetComponent<Image>();
        }
        Inventory.SetActive(false);
        
        FastAccessItemHolderImages[0].color = Color.red;
    }
    
    private void OnDropItem(int index)
    {
        ItemImages[index].sprite = null;
        ItemImages[index].color = Color.clear;
    }


    private void OnPickUpItem(int index)
    {
        Debug.Log(ItemImages[index]);
        var sprite = player.Inventory.SelectedItem.GetItemAvatarSprite();
        if (sprite != null)
        {
            ItemImages[index].sprite = sprite;
            ItemImages[index].color = Color.white;
        }
    }

    private void OnCnangeIndex(int lastIndex, int index)
    {
        if (0 <= lastIndex && lastIndex < fastAccessCapacity
            && 0 <= index && index < fastAccessCapacity)
        {
            FastAccessItemHolderImages[lastIndex].color = Color.white;
            FastAccessItemHolderImages[index].color = Color.red;
        }
    }
}
