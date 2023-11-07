using PlayerData;
using UnityEngine;
using UnityEngine.Events;

namespace Utils
{
    public class ActionOnEnable : MonoBehaviour
    {
        [SerializeField] private UnityEvent action;
        
        private void OnEnable()
        {
            action.Invoke();
        }
    }
}
