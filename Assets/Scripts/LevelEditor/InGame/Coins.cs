using UnityEngine;

namespace LevelEditor.InGame
{
    public class Coins : MonoBehaviour
    {
        public int value;

        private void OnTriggerEnter2D(Collider2D other)
        {
            if (other.CompareTag("Player"))
            {
                Player player = other.gameObject.GetComponent<Player>();
                player.score += value;
                player.UpdateScore();
                GetComponent<SpriteRenderer>().enabled = false;
                GetComponent<BoxCollider2D>().enabled = false;
            }
        }
    }
}
