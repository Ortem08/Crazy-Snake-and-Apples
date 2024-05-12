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
    
    private WeaponInsideInventory[] WeaponHolders;
    [SerializeField] 
    private GameObject cardHolderPrefab; 
    
    // [SerializeField] private GameObject Player;
    private PlayerComponent player;
    private CardInventory cardInventory => player.CardInventory;
    private Inventory inventory => player.Inventory;
    
    private GameObject[] playerCardHolders;
    private Dictionary<GameObject, int> spriteImagesIndexes;
    private Image[] playerCardImages;
    private Image[] playerCardSpriteImages;
    
    private Image DescriptionSpriteImage;
    private TextMeshProUGUI DescriptionText;
    
    private int playerCardCapacity;
    
    private const string DefaultDescription = "Select a card to read its description.";

    private SoundController soundController;

    private void Start()
    {
        WeaponHolders = GetComponentsInChildren<WeaponInsideInventory>();
        Debug.Log($"Start WeaponHolders 0: {WeaponHolders[0]} Len: {WeaponHolders.Length} ");
        player = FindObjectOfType<PlayerComponent>();
        playerCardCapacity = player.CardInventory.Capasity;
        playerCardHolders = new GameObject[playerCardCapacity];
        spriteImagesIndexes = new Dictionary<GameObject, int>();
        
        playerCardImages = new Image[playerCardCapacity];
        playerCardSpriteImages = new Image[playerCardCapacity];
        
        DescriptionSpriteImage = DescriptionPlaceHolder.GetComponentsInChildren<Image>()[2];
        DescriptionText = DescriptionPlaceHolder.GetComponentInChildren<TextMeshProUGUI>();
        DescriptionText.text = DefaultDescription;
        
        for (int i = 0; i < playerCardCapacity; i++)
        {
            playerCardHolders[i] = Instantiate(cardHolderPrefab, PlayerInventoryPlaceHolder.transform);
            
            var clickHandler = playerCardHolders[i].GetComponentInChildren<ClickHandler>();
            clickHandler.OnClick.AddListener(SetDescription);
            
            var dropHandler = playerCardHolders[i].GetComponentInChildren<DropHandler>();
            dropHandler.OnSwap.AddListener(SwapCards);
            
            var images =  playerCardHolders[i].GetComponentsInChildren<Image>();
            playerCardImages[i] = images[0];
            playerCardSpriteImages[i] = images[1];
            
            spriteImagesIndexes[playerCardSpriteImages[i].gameObject] = i;
        }

        soundController = GameObject.FindGameObjectWithTag("SoundController").GetComponent<SoundController>();
        
        gameObject.SetActive(false);
    }
    
    public void OpenInventory()
    {
        soundController.PlaySound("CloseInventory", 0.5f, player.gameObject.transform.position);
        
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
            SetSprite(i);
        }
    }
    
    public void CloseInventory()
    {
        soundController.PlaySound("CloseInventory", 0.5f, transform.position, player.gameObject);
        
        gameObject.SetActive(false);
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
        player.gameObject.SetActive(true);

        SingletonInputManager.instance.InputMap.Enable();
        
        Time.timeScale = 1f;
    }

    public void SetDescription(GameObject item)
    {
        ICard card = null;
        if (!spriteImagesIndexes.TryGetValue(item, out var index))
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

    public void SwapCards(GameObject first, GameObject second)
    {
        var isSwappingFirst = TryGetSwapInfo(first,
            out var imageFirst, out var inventoryFirst, out var indexFirst);
        var isSwappingSecond = TryGetSwapInfo(second,
            out var imageSecond, out var inventorySecond, out var indexSecond);
        if (!isSwappingFirst || !isSwappingSecond)
        {
            throw new ArgumentException($"Can't swap cards {first.name} and {second.name}.");
        }
        
        var tempItem = inventoryFirst.Cards[indexFirst];
        inventoryFirst.Cards[indexFirst] = inventorySecond.Cards[indexSecond];
        inventorySecond.Cards[indexSecond] = tempItem;

        SetSprite(imageFirst, inventoryFirst.Cards[indexFirst]);
        SetSprite(imageSecond, inventorySecond.Cards[indexSecond]);
    }

    private bool TryGetSwapInfo(GameObject item, 
        out Image image, out CardInventory inventory, out int index)
    {
        image = null;
        inventory = null;
        if (!spriteImagesIndexes.TryGetValue(item, out index))
        {
            foreach (var weapon in WeaponHolders)
            {
                if (weapon.TryGetSwapInfo(item, out image, out inventory, out index))
                {
                    return true;
                }
            }
        }
        else
        {
            inventory = cardInventory;
            image = playerCardSpriteImages[index];
            return true;
        }

        return false;
    }

    private void SetSprite(int i)
    {
        SetSprite(playerCardSpriteImages[i], cardInventory.Cards[i]);
    }
    
    public static void SetSprite(Image image, ICard card)
    {
        if (card is not null)
        {
            image.sprite = card.Sprite;
            image.color = Color.white;
        }
        else if (image.sprite is not null)
        {
            image.sprite = null;
            image.color = Color.clear;
        }
    }
}
