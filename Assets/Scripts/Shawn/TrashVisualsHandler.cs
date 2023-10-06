using System.ComponentModel;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Serialization;
using UnityEngine.UI;

namespace Shawn.Scripts
{
    public class TrashVisualsHandler : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
    {
        [Category("Dependencies")]
        public BuildingManager bm;
        
        [Category("Parameters")]
        public GameObject[] buttonsToDeactivate;
        public GameObject disappearFXPrefab;
        public bool invisibleOnStart;

        private Image img;
        [SerializeField] private Image childImg;

        private bool holding;
        private bool bm_couldPlace;
        private bool objectInTrash;
        private Camera mainCamera;

        // Start is called before the first frame update
        void Awake()
        {
            mainCamera = Camera.main;
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
                Vector3 pos = mainCamera.ScreenToWorldPoint(Input.mousePosition);
                OnObjectLetGo();
                if(!bm_couldPlace)
                    Instantiate(disappearFXPrefab, new Vector3(pos.x, pos.y, 0), Quaternion.identity, transform.parent);
                holding = false;
            }
            
            bm_couldPlace = pendingExists && bm.canPlace && !objectInTrash;
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

        public void OnPointerEnter(PointerEventData eventData)
        {
            objectInTrash = true;
            Debug.Log("Entered!");
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            objectInTrash = false;
            Debug.Log("Exited!");
        }
    }
}
