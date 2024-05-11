using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class UISlot : MonoBehaviour, IDropHandler
{
    private Image itemImage;
    
    private void Start()
    {
        itemImage = GetComponentsInChildren<Image>()[1];
    }
    public void OnDrop(PointerEventData eventData)
    {
        // var k = eventData.pointerDrag.
        // var tempImage = itemImage;
        // itemImage.sprite = draggedItemImage.sprite;
        // draggedItemImage.sprite = tempImage.sprite;
        //
        // if (itemImage.sprite is not null)
        //     return;
        
        var otherItemTransform = eventData.pointerDrag.transform;
        otherItemTransform.SetParent(transform);
        otherItemTransform.position = transform.position; 
    }
}
