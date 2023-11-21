using TMPro;
using UnityEngine;

namespace LevelEditor.Network
{
    public class StudentButton : MonoBehaviour
    {
        public TeacherNetworkManager managerRef;
        [HideInInspector] public int id;
        public TMP_Text username;
        public TMP_Text timestamp;
        private void Start()
        {
            //username.text = managerRef.groupData[id].username;
            //timestamp.text = managerRef.groupData[id].levelData.timestamp;
        }

        public void LoadButton()
        {
            managerRef.LoadLevel(id);
        }
    }
}
