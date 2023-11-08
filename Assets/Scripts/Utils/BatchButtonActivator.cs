using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Utils
{
    public class BatchButtonActivator : MonoBehaviour
    {
        [SerializeField] private List<GameObject> objects = new();

        public void SetAllElementsInteractable(bool interactable)
        {
            foreach (var obj in objects)
            {
                foreach (var button in obj.GetComponentsInChildren<Button>())
                {
                    button.interactable = interactable;
                }

                foreach (var image in obj.GetComponentsInChildren<Image>())
                {
                    image.raycastTarget = interactable;
                }
                
                foreach (var tmp in obj.GetComponentsInChildren<TextMeshProUGUI>())
                {
                    tmp.raycastTarget = interactable;
                }
            }
        }
    }
}
