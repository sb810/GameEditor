using UnityEngine;

namespace LevelEditor.InGame
{
    public class Checkpoint : MonoBehaviour
    {
        private void OnEnable()
        {
            GetComponent<SpriteRenderer>().color = Color.white;
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            if (other.CompareTag("Player"))
            {
                Player player = other.gameObject.GetComponent<Player>();
                player.lastCheckpoint = transform.position;
                GetComponent<SpriteRenderer>().color = Color.green;
            }
        }
    }
}
