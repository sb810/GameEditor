using PlayerData;
using UnityEngine;
using UnityEngine.Events;

namespace Utils
{
    public class OnEnableIfTeacher : MonoBehaviour
    {
        [SerializeField] private UnityEvent action;
        
        private void OnEnable()
        {
            if(PlayerDataManager.Data.isTeacher)
                action.Invoke();
        }
    }
}
