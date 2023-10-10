using System.ComponentModel;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Serialization;
using UnityEngine.UI;

namespace Shawn.Scripts
{
    public class TrashVisualsHandler : MonoBehaviour
    {
        [Category("Dependencies")]
        public BuildingManager bm;
        
        [Category("Parameters")]
        public GameObject[] buttonsToDeactivate;
        public bool invisibleOnStart;

        private Image img;
        [SerializeField] private Image childImg;

        private bool holding;

        // Start is called before the first frame update
        void Awake()
        {
            img = GetComponent<Image>();
            if (invisibleOnStart)
            {
                img.enabled = false;
                childImg.enabled = false;
            }
        }

        // Update is called once per frame
        void Update()
        {
            bool pendingExists = bm.pendingObj != null;
            
            if (pendingExists && !holding)
            {
                OnObjectHeld();
                holding = true;
            }
            else if(!pendingExists && holding)
            {
                OnObjectLetGo();
                holding = false;
            }
        }

        void OnObjectLetGo()
        {
            if (invisibleOnStart)
            {
                img.enabled = false;
                childImg.enabled = false;
                foreach (GameObject o in buttonsToDeactivate)
                {
                    o.SetActive(true);
                }
            }
        }

        void OnObjectHeld()
        {
            if (invisibleOnStart)
            {
                img.enabled = true;
                childImg.enabled = true;
                foreach (GameObject o in buttonsToDeactivate)
                {
                    o.SetActive(false);
                }
            }
        }
    }
}
