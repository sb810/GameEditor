using UnityEngine;

namespace CodingExercises
{
    public class SubCheckPoint : MonoBehaviour
    {
        private void OnTriggerEnter2D(Collider2D other)
        {
            if (!other.CompareTag("DeadBody")) return;
            transform.parent.GetComponent<PreviewCheckpoint>().CollectCheckpoint();
            GetComponent<SpriteRenderer>().enabled = false;
            GetComponentInChildren<Animator>().gameObject.SetActive(true);
        }
    }
}
