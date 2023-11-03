using UnityEngine;

namespace CodingExercises
{
    public class PreviewMeleeAttack : MonoBehaviour
    {
        [SerializeField] private BlockCodeCheck codeCheck;
        [SerializeField] private PreviewCheckpoint checkpoint;
        private static readonly int IsDeadAnimHash = Animator.StringToHash("IsDead");
        private static readonly int HurtAnimHash = Animator.StringToHash("Hurt");

        private void OnTriggerEnter2D(Collider2D collision)
        {
            if(collision.CompareTag("Player"))
            {
                collision.gameObject.GetComponent<Animator>().SetBool(IsDeadAnimHash, true);
                collision.gameObject.GetComponent<Animator>().SetTrigger(HurtAnimHash);
                collision.enabled = false;
                //collision.gameObject.SetActive(false);
                checkpoint.enemyCount--;
            }
            else if (collision.CompareTag("Enemy"))
            {
                collision.gameObject.GetComponent<Animator>().SetBool(IsDeadAnimHash, true);
                collision.gameObject.GetComponent<Animator>().SetTrigger(HurtAnimHash);
                GetComponentInParent<Animator>().SetBool(IsDeadAnimHash, true);
                GetComponentInParent<PreviewBoss>().StopAllCoroutines();
                codeCheck.StopAllCoroutines();
                codeCheck.UnhighlightAllBlocks();
                collision.enabled = false;
                //collision.gameObject.SetActive(false);
                // codeCheck.ResetPreview();
            }
        }
    }
}
