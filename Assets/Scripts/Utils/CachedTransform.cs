using UnityEngine;

namespace Utils
{
    public class CachedTransform : MonoBehaviour
    {
        public Vector2 AnchoredPosition { get; private set; }
        public Vector2 sizeDelta { get; private set; }
        private RectTransform rt;

        private void Awake()
        {
            rt = GetComponent<RectTransform>();
            ApplyAll();
        }

        public void ApplyPosition()
        {
            AnchoredPosition = rt.anchoredPosition;
        }
        
        
        public void ApplySizeDelta()
        {
            sizeDelta = rt.sizeDelta;
        }

        public void ApplyAll()
        {
            ApplyPosition();
            ApplySizeDelta();
        }
    }
}
