using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Network
{
    public class ProfMdp : MonoBehaviour
    {
        public Button button;
        public string password;

        private void Start()
        {
            if (PlayerPrefs.HasKey("isProf"))
            {
                button.interactable = false;
            }
        }
        public void CheckMdp(string mdp)
        {
            if(mdp == password)
            {
                button.interactable = false;
                PlayerPrefs.SetInt("isProf", 1);
                GetComponent<TMP_InputField>().text = "";
            }
            else
            {
                GetComponent<TMP_InputField>().text = "";
            }
        }
    }
}
