using UnityEngine;

namespace LevelEditor.InGame
{
    public class Bub : MonoBehaviour
    {
        public float moveSpeed = 1f;
        public LayerMask ground;
        public LayerMask ennemi;

        private Rigidbody2D rb;
        public Collider2D triggerColliderGround;
        public Collider2D triggerColliderWall;
        private static readonly int IsWalkingAnimHash = Animator.StringToHash("IsWalking");

        private void Start()
        {
            rb = GetComponent<Rigidbody2D>();
            GetComponent<Animator>().SetBool(IsWalkingAnimHash, true);
        }

        private void Update()
        {
            rb.velocity = new Vector2(moveSpeed, rb.velocity.y);
        }

        private void FixedUpdate()
        {
            if (!triggerColliderGround.IsTouchingLayers(ground) || triggerColliderWall.IsTouchingLayers(ennemi))
            {
                Flip();
            }
        }

        private void Flip()
        {
            transform.localScale = new Vector2(transform.localScale.x * -1, transform.localScale.y);
            moveSpeed *= -1;
        }
    }
}
