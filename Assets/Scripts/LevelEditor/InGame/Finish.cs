using UnityEngine;

namespace LevelEditor.InGame
{
    public class Finish : MonoBehaviour
    {
        private void OnTriggerEnter2D(Collider2D other)
        {
            if (other.CompareTag("Player"))
            {
                GameManager.Instance.BuildingManager.Stop();
            }
        }
    }
}
