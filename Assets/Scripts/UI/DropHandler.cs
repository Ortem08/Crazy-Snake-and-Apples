using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class DropHandler : MonoBehaviour, IDropHandler
{
    private GameObject cardChildren;
    // private static Dictionary<GameObject, Image> itemImages = new Dictionary<GameObject, Image>();
    public UnityEvent<GameObject, GameObject> OnSwap { get; } = new();


    private void Awake()
    {
        cardChildren = GetComponentsInChildren<Image>()[1].gameObject;
    }

    public void OnDrop(PointerEventData eventData)
    {
        // var tempImage = itemImage?.sprite;
        // if (itemImages.TryGetValue(eventData.pointerDrag, out var image))
        // {
        //     itemImage.sprite = itemImages[eventData.pointerDrag]?.sprite;
        // }
        // else
        // {
        //     itemImage.sprite = null;
        // }
        // itemImages[eventData.pointerDrag].sprite = tempImage;
        
        OnSwap.Invoke(eventData.pointerDrag, cardChildren);
        
        // var otherItemTransform = eventData.pointerDrag.transform;
        // otherItemTransform.SetParent(transform);
        // otherItemTransform.position = transform.position; 
    }
}
