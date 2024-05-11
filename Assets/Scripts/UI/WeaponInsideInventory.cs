using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class WeaponInsideInventory : MonoBehaviour
{
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
    
    void Awake()
    {
        ItemHolders = new GameObject[Capacity];
        holderIndexes = new Dictionary<GameObject, int>();
        ItemImages = new Image[Capacity];
        FastAccessItemHolderImages = new Image[Capacity];
        
        for (var i = 0; i < Capacity; i++)
        {
            ItemHolders[i] = Instantiate(itemHolderPrefab, CardsHolder.transform);
            holderIndexes[ItemHolders[i]] = i;
            
            var images =  ItemHolders[i].GetComponentsInChildren<Image>();
            FastAccessItemHolderImages[i] = images[0];
            ItemImages[i] = images[1];  
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
                if (inventory.Cards[i] != null)
                {
                    SetCardSprite(i);
                }
                else
                {
                    ItemImages[i].color = Color.clear;
                    ItemImages[i].sprite = null;;
                }
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
        var sprite = inventory.Cards[index].Sprite;
        if (sprite is not null)
        {
            ItemImages[index].color = Color.white;
            ItemImages[index].sprite = sprite;
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
