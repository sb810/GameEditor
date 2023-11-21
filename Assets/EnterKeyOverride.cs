using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class EnterKeyOverride : MonoBehaviour
{
    [SerializeField] private UnityEvent onKeyPressed; 
    
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Return))
        {
            onKeyPressed.Invoke();
        }
    }
}
