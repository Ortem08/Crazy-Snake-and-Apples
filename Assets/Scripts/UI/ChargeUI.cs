using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class NewBehaviourScript : MonoBehaviour
{
    [SerializeField] 
    private GameObject holder;
    
    [SerializeField] 
    private TextMeshProUGUI remaining;
    
    [SerializeField] 
    private TextMeshProUGUI full;

    [SerializeField] 
    private FastAccessInventoryUI fastInventory;

    private Inventory playerInventory { get; set; }
    private IChargeable selectedItem { get; set; }
    
    private void Start()
    {
        var player = FindObjectOfType<PlayerComponent>();
        playerInventory = player.Inventory;
        playerInventory.OnCnangeIndex.AddListener(OnCnangeIndex);
        playerInventory.OnPickUpItem.AddListener(OnPickUpItem);
        holder.SetActive(false);
    }
    
    private void OnPickUpItem(int index)
    {
        selectedItem = playerInventory.Items[index] as IChargeable;
        if (selectedItem is null)
        {
            holder.SetActive(false);
            return;
        }

        holder.SetActive(true);
        remaining.text = selectedItem.ChargeInfo.CurrentCharge.ToString();
        full.text = selectedItem.ChargeInfo.MaxCharge.ToString();
        selectedItem.OnChargeChanged += OnChargeChanged;
    }

    private void OnCnangeIndex(int lastIndex, int index)
        => OnPickUpItem(index);
    
    private void OnChargeChanged(ChargeInfo chargeInfo)
    {
        remaining.text = selectedItem.ChargeInfo.CurrentCharge.ToString();
        full.text = selectedItem.ChargeInfo.MaxCharge.ToString();
    }
}