using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

public class PointerEventHandler : MonoBehaviour,
    IPointerEnterHandler, IPointerExitHandler, IPointerDownHandler, IPointerUpHandler, IPointerMoveHandler,
    IBeginDragHandler, IEndDragHandler
{
    [SerializeField] private UnityEvent pointerEnterEvent;
    [SerializeField] private UnityEvent pointerExitEvent;
    [SerializeField] private UnityEvent pointerDownEvent;
    [SerializeField] private UnityEvent pointerUpEvent;
    [SerializeField] private UnityEvent pointerMoveEvent;
    [SerializeField] private UnityEvent dragEvent;
    [SerializeField] private UnityEvent dropEvent;

    public void OnPointerEnter(PointerEventData eventData)
    {
        pointerEnterEvent.Invoke();
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        pointerExitEvent.Invoke();
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        pointerDownEvent.Invoke();
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        pointerUpEvent.Invoke();
    }

    public void OnPointerMove(PointerEventData eventData)
    {
        pointerMoveEvent.Invoke();
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        dragEvent.Invoke();
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        dropEvent.Invoke();
    }
}