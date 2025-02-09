using System.Collections;
using UnityEngine;

namespace LevelEditor.InGame
{
    public class UnstablePlatform : MonoBehaviour
    {
        public float delay;
        private void OnEnable()
        {
            GetComponent<Rigidbody2D>().bodyType = RigidbodyType2D.Kinematic;
            GetComponent<Collider2D>().enabled = true;
        }

        private void OnCollisionEnter2D(Collision2D collision)
        {
            if (collision.gameObject.CompareTag("Player"))
            {
                StartCoroutine(Fall());
            }
        }

        private IEnumerator Fall()
        {
            yield return new WaitForSeconds(delay);
            GetComponent<Rigidbody2D>().bodyType = RigidbodyType2D.Dynamic;
            GetComponent<Collider2D>().enabled = false;
            yield return null;
        }
    }
}

