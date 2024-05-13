using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FastAccessInventoryUI : MonoBehaviour
{
    private GameObject player;
    private PlayerComponent playerComponent;
    
    [SerializeField] 
    private Image[] FastAccessItemHolderImages;
    private GameObject[] ItemHolders;
    private Image[] ItemImages;
    
    [SerializeField]
    private GameObject itemHolderPrefab;

    private SoundController soundController;
   
    public int totalCapacity { get; private set; }
    
    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");
        playerComponent = player.GetComponent<PlayerComponent>();
        
        Debug.Log($"Player in Inventory: {playerComponent}");
        LoadInventory();
        playerComponent.Inventory.OnCnangeIndex.AddListener(OnCnangeIndex);
        playerComponent.Inventory.OnPickUpItem.AddListener(OnPickUpItem);
        playerComponent.Inventory.OnDropItem.AddListener(OnDropItem);

        soundController = GameObject.FindGameObjectWithTag("SoundController").GetComponent<SoundController>();
    }

    private void LoadInventory()
    {
        totalCapacity = playerComponent.Inventory.Capacity;
        ItemHolders = new GameObject[totalCapacity];
        ItemImages = new Image[totalCapacity];
        FastAccessItemHolderImages = new Image[totalCapacity];
        
        for (var i = 0; i < totalCapacity; i++)
        {
            ItemHolders[i] = Instantiate(itemHolderPrefab, transform);
            var images =  ItemHolders[i].GetComponentsInChildren<Image>();
            FastAccessItemHolderImages[i] = images[0];
            ItemImages[i] = images[1];
        }
        
        FastAccessItemHolderImages[playerComponent.Inventory.SelectedIndex].color = Color.red;
    }
    
    private void OnDropItem(int index)
    {
        ItemImages[index].sprite = null;
        ItemImages[index].color = Color.clear;
        soundController.PlaySound("Throw", 0.5f, 2, transform.position, player);
    }
    
    private void OnPickUpItem(int index)
    {
        // Debug.Log(ItemImages[index]);
        var sprite = playerComponent.Inventory.SelectedItem.GetItemAvatarSprite();
        if (sprite != null)
        {
            ItemImages[index].sprite = sprite;
            ItemImages[index].color = Color.white;
            soundController.PlaySound("PickUp", 0.5f, 1, transform.position, player);
        }
    }

    private void OnCnangeIndex(int lastIndex, int index)
    {
        if (0 <= lastIndex && lastIndex < totalCapacity
            && 0 <= index && index < totalCapacity)
        {
            FastAccessItemHolderImages[lastIndex].color = Color.white;
            FastAccessItemHolderImages[index].color = Color.red;
            soundController.PlaySound("ChangeHand", 0.5f, 1, transform.position, player);
        }
    }
}
