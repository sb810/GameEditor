using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class MouseEventHandler : MonoBehaviour
{
    
    public UnityEvent mouseEnterEvent;
    public UnityEvent mouseExitEvent;
    public UnityEvent mouseDownEvent;
    public UnityEvent mouseUpEvent;
    
    private void OnMouseEnter()
    {
        mouseEnterEvent.Invoke();
    }

    private void OnMouseExit()
    {
        mouseExitEvent.Invoke();
    }

    private void OnMouseDown()
    {
        mouseDownEvent.Invoke();
    }

    private void OnMouseUp()
    {
        mouseUpEvent.Invoke();
    }
}
