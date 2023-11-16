using UnityEngine;

namespace CodingExercises
{
    public class PreviewBoss : MonoBehaviour
    {
        public LayerMask ground;
        public Collider2D triggerCollider;
        [HideInInspector]public bool hole;
        [HideInInspector]public bool seePlayer;
        [HideInInspector]public int facing = 1;

        public GameObject fireball;
        public GameObject mouth;
        [SerializeField] private BoxCollider2D attackHitbox;
        [SerializeField] private float playerDetectionDistance = 2.5f;
        
        private void FixedUpdate()
        {
            Vector2 origin = transform.position + Vector3.up / 2;
            Vector2 direction = Vector2.left * facing;
            
            #if UNITY_EDITOR
            Debug.DrawRay(origin, direction * playerDetectionDistance);
            #endif
            
            RaycastHit2D[] hits = Physics2D.RaycastAll(origin, direction, playerDetectionDistance);
            bool playerFound = false;
            foreach (var hit in hits)
            {
                if (hit.collider != null && hit.collider.CompareTag("Player")) playerFound = true;
            }

            seePlayer = playerFound;
            hole = !triggerCollider.IsTouchingLayers(ground);
        }

        public void AttackRanged()
        {
            GameObject obj = Instantiate(fireball, mouth.transform.position, Quaternion.identity);
            obj.GetComponent<PreviewFirebal>().speed *= facing;
        }

        public void StartMeleeAttack()
        {
            attackHitbox.enabled = true;
        }

        public void EndMeleeAttack()
        {
            attackHitbox.enabled = false;
        }
    }
}
