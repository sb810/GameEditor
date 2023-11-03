using UnityEngine;

namespace UI
{
    public class MouseIconHandler : MonoBehaviour
    {
        public static MouseIconHandler Instance { get; private set; }

        [SerializeField] private Texture2D cursorDefault;
        [SerializeField] private Texture2D cursorPointer;
        [SerializeField] private Texture2D cursorHandOpen;
        [SerializeField] private Texture2D cursorHandHold;
        
        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(this);
            }
            else
            {
                Instance = this;
            }
        }
        
        private bool cursorLock = false;

        public void SetCursorLock(bool locked)
        {
            cursorLock = locked;
        }


        // Start is called before the first frame update
        private void Start()
        {
            Cursor.visible = true;
            SetCursorDefault();
        }
        
        private delegate void NextCursorFunction();
        private NextCursorFunction eNextCursorFunction;

        public void SetCursorDefault()
        {
            if(!cursorLock)
                Cursor.SetCursor(cursorDefault, Vector2.zero, CursorMode.Auto);
        }
        
        public void SetCursorPointer()
        {
            if(!cursorLock)
                Cursor.SetCursor(cursorPointer, Vector2.zero, CursorMode.Auto);
        }
        
        public void SetCursorHandOpen()
        {
            if(!cursorLock)
                Cursor.SetCursor(cursorHandOpen, Vector2.zero, CursorMode.Auto);
        }
        
        public void SetCursorHandHold()
        {
            if(!cursorLock)
                Cursor.SetCursor(cursorHandHold, Vector2.zero, CursorMode.Auto);
        }
        
        public void SetCursorDefaultQueued()
        {
            eNextCursorFunction = SetCursorDefault;
        }
        
        public void SetCursorPointerQueued()
        {
            eNextCursorFunction = SetCursorPointer;
        }
        
        public void SetCursorHandOpenQueued()
        {
            eNextCursorFunction = SetCursorHandOpen;
        }
        
        public void SetCursorHandHoldQueued()
        {
            eNextCursorFunction = SetCursorHandHold;
        }

        private void Update()
        {
            if (cursorLock || eNextCursorFunction == null) return;
            eNextCursorFunction.Invoke();
            eNextCursorFunction = null;
        }
    }
}
