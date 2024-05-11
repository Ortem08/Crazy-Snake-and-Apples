using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class UIItem : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
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
