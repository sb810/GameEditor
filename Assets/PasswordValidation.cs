using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;

public class PasswordValidation : MonoBehaviour
{
    [SerializeField] private string password;
    [SerializeField] private UnityEvent onPasswordValid;
    [SerializeField] private UnityEvent onPasswordInvalid;
    
    public void Validate(TMP_InputField input)
    {
        if (input.text == password)
            onPasswordValid.Invoke();
        else onPasswordInvalid.Invoke();
    }
}
