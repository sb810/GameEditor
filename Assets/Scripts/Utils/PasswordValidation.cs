using System;
using PlayerData;
using TMPro;
using UnityEngine;
using UnityEngine.Events;

namespace Utils
{
    public class PasswordValidation : MonoBehaviour
    {
        [SerializeField] private UnityEvent onPasswordValid;
        [SerializeField] private UnityEvent onPasswordInvalid;

        public void ValidateDesignPassword(TMP_InputField input)
        {
            if (input.text == PlayerDataManager.DesignPassword)
                onPasswordValid.Invoke();
            else onPasswordInvalid.Invoke();
        }
        
        public void ValidateCodingPassword(TMP_InputField input)
        {
            if (input.text == PlayerDataManager.CodingPassword)
                onPasswordValid.Invoke();
            else onPasswordInvalid.Invoke();
        }
    }
}
