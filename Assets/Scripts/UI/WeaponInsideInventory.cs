using System.Collections;
using System.Collections.Generic;
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
    private Image[] ItemImages;
    
    [SerializeField]
    private GameObject itemHolderPrefab;

    public int Capacity => inventory?.Capasity ?? 0;
    
    void Start()
    {
        // inventory.OnPickUpCard.AddListener(OnPickUpCard);
        // inventory.OnDropCard.AddListener(OnDropCard);
    }
    
    public void ReloadInventory(ICardBasedItem weaponCardBased)
    {
        if (weapon != null && weaponCardBased != null 
            && Capacity == weaponCardBased.CardInventory.Capasity)
        {
            gameObject.SetActive(true);
            weapon = weaponCardBased;
            SetWeaponSprite();
            for (var i = 0; i < Capacity; i++)
            {
                if (inventory.Cards[i] != null)
                {
                    SetCardSprite(i);
                }
            }

            return;
        }
        
        if (weapon is not null && weaponCardBased is null)
        {
            gameObject.SetActive(false);
            
            WeaponIcon.color = Color.clear;
            
            // Раскомментировать, если количество карт в инвентаре оружия != 20
            // for (var i = 0; i < Capacity; i++)
            // {
            //     Destroy(ItemHolders[i]);
            // }
            
            weapon = null;
            
            return;
        }
        
        ChangedCapacityReload(weaponCardBased);
    }

    private void ChangedCapacityReload(ICardBasedItem weaponCardBased)
    {
        gameObject.SetActive(true);
        for (var i = 0; i < Capacity; i++)
        {
            Destroy(ItemHolders[i]);
        }

        weapon = weaponCardBased;
        SetWeaponSprite();

        ItemHolders = new GameObject[Capacity];
        ItemImages = new Image[Capacity];
        FastAccessItemHolderImages = new Image[Capacity];
        
        for (var i = 0; i < Capacity; i++)
        {
            ItemHolders[i] = Instantiate(itemHolderPrefab, CardsHolder.transform);
            var images =  ItemHolders[i].GetComponentsInChildren<Image>();
            FastAccessItemHolderImages[i] = images[0];
            ItemImages[i] = images[1];
            if (inventory.Cards[i] != null)
            {
                SetCardSprite(i);
            }
        }
    }
    
    // private void OnDropCard(int index)
    // {
    //     ItemImages[index].sprite = null;
    //     ItemImages[index].color = Color.clear;
    // }


    private void SetCardSprite(int index)
    {
        var sprite = inventory.Cards[index].Sprite;
        if (sprite is not null)
        {
            ItemImages[index].sprite = sprite;
            ItemImages[index].color = Color.white;
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
    
    // private void OnPickUpCard(int index)
    //     => SetCardSprite(index);
}
