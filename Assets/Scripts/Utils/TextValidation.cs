using System.Text.RegularExpressions;
using TMPro;
using UnityEngine;
using UnityEngine.Events;

namespace Utils
{
    public class TextValidation : MonoBehaviour
    {
        [SerializeField] private bool preventEmpty = true;
        [SerializeField] private UnityEvent onTextValid;
        [SerializeField] private UnityEvent onTextInvalid;

        public void Validate(string input)
        {
            if (preventEmpty)
            {
                if (Regex.IsMatch(input, "^\\s*$"))
                    onTextInvalid.Invoke();
                else onTextValid.Invoke();
            } else onTextValid.Invoke();
        }
        
        public void Validate(TMP_InputField input)
        {
            Validate(input.text);
        }
    }
}
