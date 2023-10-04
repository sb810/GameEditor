using System;
using System.Collections;
using System.Collections.Generic;
using Sirenix.Serialization;
using UnityEngine;
public class MouseIconHandler : MonoBehaviour
{
    [SerializeField] private Texture2D cursorDefault;
    [SerializeField] private Texture2D cursorPointer;
    [SerializeField] private Texture2D cursorHandOpen;
    [SerializeField] private Texture2D cursorHandHold;

    private bool cursorLock = false;

    public void SetCursorLock(bool locked)
    {
        cursorLock = locked;
    }

    public enum CursorState
    {
        Default,
        Pointer,
        HandOpen,
        HandHold
    }
    
    // Start is called before the first frame update
    void Start()
    {
        Cursor.visible = true;
        Cursor.SetCursor(cursorDefault, Vector2.zero, CursorMode.Auto);
    }

    public void ChangeCursor(CursorState e)
    {
        if (cursorLock) return;
        
        switch (e)
        {
            case CursorState.Default: Cursor.SetCursor(cursorDefault, Vector2.zero, CursorMode.Auto);
                break;
            case CursorState.Pointer: Cursor.SetCursor(cursorPointer, Vector2.zero, CursorMode.Auto);
                break;
            case CursorState.HandOpen: Cursor.SetCursor(cursorHandOpen, Vector2.zero, CursorMode.Auto);
                break;
            case CursorState.HandHold: Cursor.SetCursor(cursorHandHold, Vector2.zero, CursorMode.Auto);
                break;
        }
    }

    public void ChangeCursor(GetMouseButtonEnum e)
    {
        if (cursorLock) return;
        
        switch (e.state)
        {
            case CursorState.Default: Cursor.SetCursor(cursorDefault, Vector2.zero, CursorMode.Auto);
                break;
            case CursorState.Pointer: Cursor.SetCursor(cursorPointer, Vector2.zero, CursorMode.Auto);
                break;
            case CursorState.HandOpen: Cursor.SetCursor(cursorHandOpen, Vector2.zero, CursorMode.Auto);
                break;
            case CursorState.HandHold: Cursor.SetCursor(cursorHandHold, Vector2.zero, CursorMode.Auto);
                break;
        }
        
    }
}
