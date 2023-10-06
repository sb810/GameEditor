using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using Random = System.Random;

public class DragAndDropEventHandler : MonoBehaviour, 
    IPointerEnterHandler, IPointerExitHandler, IPointerDownHandler, IPointerUpHandler, IPointerMoveHandler
{
    [SerializeField] private UnityEvent pointerEnterEvent;
    [SerializeField] private UnityEvent pointerExitEvent;
    [SerializeField] private UnityEvent pointerExitQueuedEvent;
    [SerializeField] private UnityEvent pointerDownEvent;
    [SerializeField] private UnityEvent pointerUpEvent;
    [SerializeField] private UnityEvent pointerUpQueuedEvent;
    [SerializeField] private UnityEvent pointerDragEvent;

    //private bool dragging = false;
    private UnityEvent queuedEvent;
    private PointerEventData lastPointerEventData;

    public void OnPointerEnter(PointerEventData eventData)
    {
        lastPointerEventData = eventData;
        if(!eventData.dragging)
            pointerEnterEvent.Invoke();
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        lastPointerEventData = eventData;
        if (!eventData.dragging)
            pointerExitEvent.Invoke();
        else queuedEvent = pointerExitQueuedEvent;
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        lastPointerEventData = eventData;
        if(!eventData.dragging)
            pointerDownEvent.Invoke();
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        lastPointerEventData = eventData;
        if (!eventData.dragging)
            pointerUpEvent.Invoke();
        else queuedEvent = pointerUpQueuedEvent;
    }

    public void OnPointerMove(PointerEventData eventData)
    {
        lastPointerEventData = eventData;
        if(eventData.dragging)
            pointerDragEvent.Invoke();
    }

    private void Update()
    {
        if (lastPointerEventData == null) return;
        if (queuedEvent == null || lastPointerEventData.dragging) return;
        queuedEvent.Invoke();
        queuedEvent = null;
    }
}
