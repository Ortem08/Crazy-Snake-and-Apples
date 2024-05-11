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
    private Dictionary<GameObject, int> holderIndexes;
    private ClickHandler[] clickHandlers;
    private Image[] playerCardImages;
    private Image[] playerCardSpriteImages;
    
    private Image DescriptionSpriteImage;
    private TextMeshProUGUI DescriptionText;
    
    private int playerCardCapacity;
    private bool isOpened;

    private const string DefaultDescription = "Select a card to read its description.";

    private void Awake()
    {
        gameObject.SetActive(false);
    }
    

    private void FirstOpen()
    {
        player = FindObjectOfType<PlayerComponent>();
        
        playerCardCapacity = player.CardInventory.Capasity;
        playerCardHolders = new GameObject[playerCardCapacity];
        holderIndexes = new Dictionary<GameObject, int>();
        
        playerCardImages = new Image[playerCardCapacity];
        playerCardSpriteImages = new Image[playerCardCapacity];
        clickHandlers = new ClickHandler[playerCardCapacity];
        
        DescriptionSpriteImage = DescriptionPlaceHolder.GetComponentsInChildren<Image>()[2];
        DescriptionText = DescriptionPlaceHolder.GetComponentInChildren<TextMeshProUGUI>();
        DescriptionText.text = DefaultDescription;
        
        for (int i = 0; i < playerCardCapacity; i++)
        {
            playerCardHolders[i] = Instantiate(cardHolderPrefab, PlayerInventoryPlaceHolder.transform);
            holderIndexes[playerCardHolders[i]] = i;
            
            clickHandlers[i] = playerCardHolders[i].GetComponentInChildren<ClickHandler>();
            clickHandlers[i].OnClick.AddListener(SetDescription);
            
            var images =  playerCardHolders[i].GetComponentsInChildren<Image>();
            playerCardImages[i] = images[0];
            playerCardSpriteImages[i] = images[1];
        }

        isOpened = true;
    }
    
    public void OpenInventory()
    {
        if (!isOpened)
            FirstOpen();
        
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

    private void SetDescription(GameObject item)
    {
        ICard card = null;
        if (!holderIndexes.TryGetValue(item, out var index))
        {
            foreach (var weapon in WeaponHolders)
            {
                if (weapon.TrySetDescription(item, out card))
                    break;
            }
        }
        else
        {
            card = cardInventory.Cards[index];
        }
        
        if (card is null)
        {
            SetDefaultDescription();
            return;
        }
            
        DescriptionSpriteImage.sprite = card.Sprite;
        DescriptionSpriteImage.color = Color.white;
        DescriptionText.text = card.Description;
    }

    private void SetDefaultDescription()
    {
        DescriptionSpriteImage.sprite = null;
        DescriptionSpriteImage.color = Color.clear;
        DescriptionText.text = DefaultDescription;
    }
}
