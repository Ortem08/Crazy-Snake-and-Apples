using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class PlayerUI : MonoBehaviour
{
    public UnityEvent OnOpenInventory { get; } = new();
    public UnityEvent OnCloseInventory { get; } = new();
    
    [SerializeField] 
    private GameObject inventoryCanvas;
    private CardInventoryUI inventory;
    private PauseScript pause;
    void Start()
    {
        inventory = inventoryCanvas.GetComponent<CardInventoryUI>();
        pause = FindObjectOfType<PauseScript>();
    }
    
    void Update()
    {
        if (Input.GetButtonDown("Inventory"))
        {
            if (!inventoryCanvas.activeSelf && !pause.GameIsPaused)
            {
                inventory.OpenInventory();
                OnOpenInventory.Invoke();
            }
            else if (inventoryCanvas.activeSelf && pause.CanCloseInventory)
            {
                inventory.CloseInventory();
                OnCloseInventory.Invoke();
            }
        }
        
    }
}