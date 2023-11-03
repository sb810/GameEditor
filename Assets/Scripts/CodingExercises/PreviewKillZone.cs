using System;
using UnityEngine;

namespace CodingExercises
{
    public class PreviewKillZone : MonoBehaviour
    { 
        private BlockCodeCheck codeCheck;
        private static readonly int IsDeadAnimHash = Animator.StringToHash("IsDead");

        private void Awake()
        {
            codeCheck = FindObjectOfType<BlockCodeCheck>();
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            if (!other.CompareTag("DeadBody")) return;
            other.GetComponent<Animator>().SetBool(IsDeadAnimHash, true);
            other.GetComponent<PreviewBoss>().StopAllCoroutines();
            codeCheck.StopAllCoroutines();
            codeCheck.UnhighlightAllBlocks();
            gameObject.SetActive(false);
            // codeCheck.ResetPreview();
        }
    }
}
