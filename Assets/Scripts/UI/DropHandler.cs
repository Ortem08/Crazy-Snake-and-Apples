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
    public UnityEvent<GameObject, GameObject> OnSwap { get; } = new();


    private void Awake()
    {
        cardChildren = GetComponentsInChildren<Image>()[1].gameObject;
    }

    public void OnDrop(PointerEventData eventData)
    {
        OnSwap.Invoke(eventData.pointerDrag, cardChildren);
    }
}
