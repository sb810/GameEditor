using UI;
using UnityEngine;

namespace LevelEditor
{
    public class CheckPlacement : MonoBehaviour
    {
        private BuildingManager buildManager;
        public int nbLimit;

        private void Start()
        {
            buildManager = GameManager.Instance.BuildingManager;
        }
        private void OnTriggerExit2D(Collider2D other)
        {
            if (other.CompareTag("Placement"))
            {
                buildManager.canPlace = true;
            }
        }

        private void OnTriggerStay2D(Collider2D other)
        {
            if(other.CompareTag("Placement"))
            {
                buildManager.canPlace = false;
            }
        }

        private void OnMouseEnter()
        {
            MouseIconHandler.Instance.SetCursorHandOpenQueued();
        }

        private void OnMouseExit()
        {
            MouseIconHandler.Instance.SetCursorDefaultQueued();
        }

        private void OnMouseDown()
        {
            buildManager.canPlace = true;
            MouseIconHandler.Instance.SetCursorHandHold();
            MouseIconHandler.Instance.SetCursorLock(true);
        }

        private void OnMouseUp()
        {
            MouseIconHandler.Instance.SetCursorLock(false);
            MouseIconHandler.Instance.SetCursorHandOpenQueued();
        }
    }
}
