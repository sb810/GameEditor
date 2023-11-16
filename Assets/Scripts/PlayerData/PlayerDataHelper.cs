using TMPro;
using UnityEngine;

namespace PlayerData
{
    public class PlayerDataHelper : MonoBehaviour
    {
        public void SetSelectedCodingLevel(int selection)
        {
            PlayerDataManager.Data.selectedCodingLevel = selection;
        }

        public void SetUsername(TMP_InputField input)
        {
            PlayerDataManager.Data.username = input.text;
        }
    }
}
