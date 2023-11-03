using PlayerData;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace LevelEditor.Network
{
    public class TeacherPassword : MonoBehaviour
    {
        [SerializeField] private Button button;
        [SerializeField] private TMP_InputField inputField;

        private void Start()
        {
            if (!PlayerDataManager.Data.isTeacher) return;
            button.interactable = false;
            button.image.raycastTarget = false;
        }
        public void CheckPassword(string input)
        {
            inputField.text = "";
            if (input != PlayerDataManager.TeacherPassword) return;
            button.interactable = false;
            button.image.raycastTarget = false;
            inputField.gameObject.SetActive(false);
            PlayerDataManager.Data.isTeacher = true;
            PlayerDataManager.Data.designPasswordEntered = true;
            PlayerDataManager.Data.codingPasswordEntered = true;
        }
    }
}
