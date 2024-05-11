using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using GameObject = UnityEngine.GameObject;

public class WeaponInsideInventory : MonoBehaviour
{
    private CardInventoryUI playerCardInventoryUI;
        
    private ICardBasedItem weapon;
    private CardInventory inventory => weapon?.CardInventory;
    
    [SerializeField] 
    private GameObject CardsHolder;
    [SerializeField] 
    private Image WeaponIcon;
    
    [SerializeField] 
    private Image[] FastAccessItemHolderImages;
    private GameObject[] ItemHolders;
    private Dictionary<GameObject, int> holderIndexes;
    private Image[] ItemImages;
    
    [SerializeField]
    private GameObject itemHolderPrefab;

    public const int Capacity = 20;
    
    void Start()
    {
        playerCardInventoryUI = GetComponentInParent<CardInventoryUI>();
        
        ItemHolders = new GameObject[Capacity];
        holderIndexes = new Dictionary<GameObject, int>();
        ItemImages = new Image[Capacity];
        FastAccessItemHolderImages = new Image[Capacity];
        
        for (var i = 0; i < Capacity; i++)
        {
            ItemHolders[i] = Instantiate(itemHolderPrefab, CardsHolder.transform);
            
            var clickHandler = ItemHolders[i].GetComponentInChildren<ClickHandler>();
            clickHandler.OnClick.AddListener(playerCardInventoryUI.SetDescription);
            
            var dropHandler = ItemHolders[i].GetComponentInChildren<DropHandler>();
            dropHandler.OnSwap.AddListener(playerCardInventoryUI.SwapCards);
            
            var images =  ItemHolders[i].GetComponentsInChildren<Image>();
            FastAccessItemHolderImages[i] = images[0];
            ItemImages[i] = images[1];
            
            holderIndexes[ItemImages[i].gameObject] = i;
        }
        CardsHolder.SetActive(false);
    }
    
    public void ReloadInventory(ICardBasedItem weaponCardBased)
    {
        if (weaponCardBased != null)
        {
            CardsHolder.SetActive(true);
            weapon = weaponCardBased;
            SetWeaponSprite();
            for (var i = 0; i < Capacity; i++)
            {
                SetCardSprite(i);
            }

            return;
        }
        
        if (weapon is not null && weaponCardBased is null)
        {
            CardsHolder.SetActive(false);
            WeaponIcon.color = Color.clear;
            weapon = null;
            return;
        }
    }

    public bool TryGetSwapInfo(GameObject item,
        out Image image, out CardInventory inventory, out int index)
    {
        image = null;
        inventory = null;
        if (!holderIndexes.TryGetValue(item, out index))
        {        
            return false;
        }
        
        inventory = this.inventory;
        image = ItemImages[index];
        return true;
    }
    
    public bool TrySetDescription(GameObject item,
        out ICard card)
    {
        card = null;
        
        if (!holderIndexes.TryGetValue(item, out var index))
            return false;
        
        card = inventory.Cards[index];
        return true;
    }
    
    private void SetCardSprite(int index)
    {
        if (inventory.Cards[index] is not null)
        {
            ItemImages[index].color = Color.white;
            ItemImages[index].sprite = inventory.Cards[index].Sprite;
        }
        else if (ItemImages[index].sprite is not null)
        {
            ItemImages[index].sprite = null;
            ItemImages[index].color = Color.clear;
        }
    }
    
    private void SetWeaponSprite()
    {
        var sprite = weapon?.GetItemAvatarSprite();
        if (sprite is not null)
        {
            WeaponIcon.sprite = sprite;
            WeaponIcon.color = Color.white;
        }
    }
}
