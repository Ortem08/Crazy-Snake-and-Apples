using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class UIItem : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    // private Vector3 originalParent;
    // private RectTransform rectTransform;
    // private CanvasGroup canvasGroup;
    // private Canvas mainCanvas;
    //
    // private void Start()
    // {
    //     rectTransform = GetComponent<RectTransform>();
    //     canvasGroup = GetComponent<CanvasGroup>();
    //     var foundCanvas = GetComponentsInParent<Canvas>();
    //
    //     mainCanvas = foundCanvas[^1];
    //     Debug.Log(mainCanvas.name);
    // }
    //
    // public void OnBeginDrag(PointerEventData eventData)
    // {
    //     var slotTransform = rectTransform.parent;
    //     slotTransform.SetAsLastSibling();
    //     canvasGroup.blocksRaycasts = false;
    // }
    //
    // public void OnDrag(PointerEventData eventData)
    // {
    //     // rectTransform.position = eventData.position;
    //     rectTransform.anchoredPosition += eventData.delta / mainCanvas.scaleFactor;
    // }
    //
    // public void OnEndDrag(PointerEventData eventData)
    // {
    //     transform.localPosition = Vector3.zero; 
    //     canvasGroup.blocksRaycasts = true;
    // }
    
    
    private Vector2 originalPosition;
    private RectTransform rectTransform;
    private Transform originalParent;
    private CanvasGroup canvasGroup;
    private Canvas mainCanvas;
    
    private void Start()
    {
        canvasGroup = GetComponent<CanvasGroup>();
        rectTransform = GetComponent<RectTransform>();
        var foundCanvas = GetComponentsInParent<Canvas>();
        
        mainCanvas = foundCanvas[^1];
    }
    
    public void OnBeginDrag(PointerEventData eventData)
    {
        originalPosition = rectTransform.position;
        originalParent = rectTransform.parent;
        rectTransform.SetParent(rectTransform.root);  
        canvasGroup.blocksRaycasts = false;
    }
    
    public void OnDrag(PointerEventData eventData)
    {
        rectTransform.anchoredPosition += eventData.delta / mainCanvas.scaleFactor;
    }
    
    public void OnEndDrag(PointerEventData eventData)
    {
        if (rectTransform.parent == rectTransform.parent.root)
        {
            rectTransform.position = originalPosition; 
            rectTransform.SetParent(originalParent);
        }
        canvasGroup.blocksRaycasts = true;
    }
}
