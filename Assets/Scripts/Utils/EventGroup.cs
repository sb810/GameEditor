using UnityEngine;
using UnityEngine.Events;

namespace Utils
{
    public class EventGroup : MonoBehaviour
    {
        [TextArea] [SerializeField] private string description;
        [Space] [SerializeField] private UnityEvent events;

        public void InvokeAll()
        {
            events.Invoke();
        }
    }
}