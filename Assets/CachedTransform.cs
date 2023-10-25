using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CachedTransform : MonoBehaviour
{
    public Vector2 AnchoredPosition { get; private set; }

    private void Awake()
    {
        RectTransform rect = GetComponent<RectTransform>();
        AnchoredPosition = rect.anchoredPosition;
    }
}
