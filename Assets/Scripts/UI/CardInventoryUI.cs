using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class CardInventoryUI : MonoBehaviour
{
    [SerializeField] 
    private GameObject WeaponInventoryPlaceHolder;
    [SerializeField] 
    private GameObject PlayerInventoryPlaceHolder;
    [SerializeField] 
    private GameObject DescriptionPlaceHolder;
    [SerializeField] 
    private WeaponInsideInventory[] WeaponHolders;
    [SerializeField] 
    private GameObject cardHolderPrefab; 
    
    // [SerializeField] private GameObject Player;
    private PlayerComponent player;
    private CardInventory cardInventory => player.CardInventory;
    private Inventory inventory => player.Inventory;
    
    private GameObject[] playerCardHolders;
    private Image[] playerCardImages;
    private Image[] playerCardSpriteImages;
    
    private Image DescriptionSpriteImage;
    private TextMeshProUGUI DescriptionText;
    
    private int playerCardCapacity;

    private const string startDescription = "Select a card to read its description.";

    private void Start()
    {
        player = FindObjectOfType<PlayerComponent>();
        Debug.Log($"Player in Card Inventory: {player}");
        gameObject.SetActive(false);
        
        DescriptionSpriteImage = DescriptionPlaceHolder.GetComponentsInChildren<Image>()[2];
        DescriptionText = DescriptionPlaceHolder.GetComponentInChildren<TextMeshProUGUI>();
        DescriptionText.text = startDescription;
        
        playerCardCapacity = player.CardInventory.Capasity;
        playerCardHolders = new GameObject[playerCardCapacity];
        playerCardImages = new Image[playerCardCapacity];
        playerCardSpriteImages = new Image[playerCardCapacity];
        
        for (int i = 0; i < playerCardCapacity; i++)
        {
            playerCardHolders[i] = Instantiate(cardHolderPrefab, PlayerInventoryPlaceHolder.transform);
            var images =  playerCardHolders[i].GetComponentsInChildren<Image>();
            playerCardImages[i] = images[0];
            playerCardSpriteImages[i] = images[1];
        }
    }
    
    public void OpenInventory()
    {
        gameObject.SetActive(true);
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
        player.gameObject.SetActive(false);
        SingletonInputManager.instance.InputMap.Disable();
        
        Time.timeScale = 0f;
        
        for (var i = 0; i < WeaponHolders.Length; i++)
        {
            var cardBasedItem = inventory.Items[i] as ICardBasedItem;
            WeaponHolders[i].ReloadInventory(cardBasedItem);
        }
        
        for (var i = 0; i < playerCardCapacity; i++)
        {
            if (cardInventory.Cards[i] is not null)
            {
                playerCardSpriteImages[i].sprite = cardInventory.Cards[i].Sprite;
                playerCardSpriteImages[i].color = Color.white;
            }
            else if (playerCardImages[i].sprite is not null)
            {
                playerCardSpriteImages[i].sprite = null;
                playerCardSpriteImages[i].color = Color.clear;
            }
        }
    }
    
    public void CloseInventory()  
    {
        gameObject.SetActive(false);
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
        player.gameObject.SetActive(true);

        SingletonInputManager.instance.InputMap.Enable();
        
        Time.timeScale = 1f;
    }
}
