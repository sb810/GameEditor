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

        private void FixedUpdate()
        {
            RaycastHit2D hit = Physics2D.Raycast(transform.position+Vector3.up/2, Vector2.left*facing,2000);
            seePlayer = hit.collider != null && hit.collider.CompareTag("Player");
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
