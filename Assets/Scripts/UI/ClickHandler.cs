using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

public class ClickHandler : MonoBehaviour, IPointerClickHandler
{
    public UnityEvent<GameObject> OnClick { get; } = new();

    public void OnPointerClick(PointerEventData eventData)
    {
        OnClick.Invoke(eventData.pointerClick);
    }
}
