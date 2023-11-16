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
            {
                PlayerDataManager.Data.designPasswordEntered = true;
                onPasswordValid.Invoke();
            }
            else onPasswordInvalid.Invoke();
        }

        public void ValidateCodingPassword(TMP_InputField input)
        {
            if (input.text == PlayerDataManager.CodingPassword)
            {
                PlayerDataManager.Data.codingPasswordEntered = true;
                onPasswordValid.Invoke();
            }
            else onPasswordInvalid.Invoke();
        }

        public void ValidateDesignPasswordInCache()
        {
            if (PlayerDataManager.Data.isTeacher || PlayerDataManager.Data.designPasswordEntered)
                onPasswordValid.Invoke();
            else onPasswordInvalid.Invoke();
        }

        public void ValidateDesignPasswordAndNameEnteredInCache()
        {
            if (PlayerDataManager.Data.username.Length > 0 &&
                (PlayerDataManager.Data.isTeacher || PlayerDataManager.Data.designPasswordEntered))
                onPasswordValid.Invoke();
            else onPasswordInvalid.Invoke();
        }

        public void ValidateCodingPasswordInCache()
        {
            if (PlayerDataManager.Data.isTeacher || PlayerDataManager.Data.codingPasswordEntered)
                onPasswordValid.Invoke();
            else onPasswordInvalid.Invoke();
        }
    }
}