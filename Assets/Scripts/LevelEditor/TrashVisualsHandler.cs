using UnityEngine;
using UnityEngine.Events;

namespace LevelEditor
{
    public class TrashVisualsHandler : MonoBehaviour
    {
        public UnityEvent onGrab;
        public UnityEvent onDrop;

        private BuildingManager buildingManager;
        private bool holding;

        private void Awake()
        {
            buildingManager = GameManager.Instance.BuildingManager;
        }

        private void Update()
        {
            bool pendingExists = buildingManager.pendingObj != null;
            
            if (pendingExists && !holding)
            {
                onGrab.Invoke();
                holding = true;
            }
            else if(!pendingExists && holding)
            {
                onDrop.Invoke();
                holding = false;
            }
        }
    }
}
