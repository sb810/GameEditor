using System;
using UnityEngine;

namespace LevelEditor.InGame
{
    public class EnemyWeakness : MonoBehaviour
    {
        public Vector2 force;
        public Animator enemy;


        private void OnTriggerEnter2D(Collider2D other)
        {
            if (!other.CompareTag("PlayerHazard")) return;
            
            Player player = other.GetComponentInParent<Player>();
            player.Bounce(force);
            enemy.SetBool("IsDead", true);
            if(transform.parent.GetComponent<Bub>() != null)
            {
                enemy.SetTrigger("Hurt");
                transform.parent.GetComponent<Collider2D>().enabled = false;
                transform.parent.GetComponent<Rigidbody2D>().simulated = false;
                transform.parent.GetComponent<Bub>().enabled = false;
            }

            if (transform.parent.GetComponent<Boss>() != null)
            {
                transform.parent.GetComponent<Rigidbody2D>().simulated = false;
                transform.parent.GetComponent<Boss>().enabled = false;
                //transform.parent.GetComponent<Animator>().enabled = false;
            }
        }
    }
}
