using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Utils
{
    public class KeepRaycastDeactivated : MonoBehaviour
    {
        private Image img;
        private TextMeshProUGUI tmp;
        private Button btn;

        private void Start()
        {
            if (TryGetComponent(out Image img))
                this.img = img;
            if (TryGetComponent(out TextMeshProUGUI tmp))
                this.tmp = tmp;
            if (TryGetComponent(out Button btn))
                this.btn = btn;
        }

        private void Update()
        {
            if (img) img.raycastTarget = false;
            if (tmp) tmp.raycastTarget = false;
            if (btn) btn.enabled = false;
        }
    }
}
