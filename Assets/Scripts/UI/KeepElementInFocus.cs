using UnityEngine;
using UnityEngine.EventSystems;

namespace UI
{
    [RequireComponent(typeof(EventSystem))]
    public class KeepElementInFocus : MonoBehaviour
    {
        private GameObject element;

        public GameObject Element
        {
            private get => element;
            set => element = value;
        }
        private EventSystem es;
        private void Awake()
        {
            es = GetComponent<EventSystem>();
            element = es.firstSelectedGameObject;
        }

        private void Update()
        {
            if(es.currentSelectedGameObject != element)
                es.SetSelectedGameObject(element);
        }
    }
}
