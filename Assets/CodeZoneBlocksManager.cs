using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class CodeZoneBlocksManager : MonoBehaviour, IPointerMoveHandler, IPointerEnterHandler, IPointerExitHandler
{
    public Canvas canvas;
    private bool hovering;
    private float mousePositionY;
    
    public void OnPointerMove(PointerEventData eventData)
    {
        // 324 - 41 - 
        // 
        
        RectTransform rectTransform = transform as RectTransform;
        if (RectTransformUtility.ScreenPointToLocalPointInRectangle(rectTransform, eventData.position, Camera.main, out var localMousePosition))
        {
            mousePositionY = -(localMousePosition.y - 129);
        }
        
        //mousePositionY = eventData.position.y;
        Debug.Log(mousePositionY);
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        Debug.Log("hovering");
        hovering = true;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        Debug.Log("not hovering");
        hovering = false;
    }

    public int GetCurrentHoveredBlockPositionIndex()
    {
        if (!hovering) return -1;
        
        int blockHeight = 17;
        int allBlocksHeight = transform.childCount * blockHeight + 12;

        if (mousePositionY >= allBlocksHeight) return transform.childCount;
        
        int index = (int)(mousePositionY / blockHeight);
        return Mathf.Clamp(index, 0, transform.childCount - 1);
    }
}
