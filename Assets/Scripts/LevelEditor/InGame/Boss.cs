using System.Collections;
using PlayerData;
using UnityEngine;
using UnityEngine.Serialization;

namespace LevelEditor.InGame
{
    public class Boss : MonoBehaviour
    {
        public float moveSpeed = 1f;
        public LayerMask ground;
        [FormerlySerializedAs("ennemi")] public LayerMask enemy;
        
        [SerializeField] private BoxCollider2D attackHitbox;
        [SerializeField] private float playerDetectionDistance = 2.5f;

        private Rigidbody2D rb;
        private Animator anim;
        public Collider2D triggerColliderGround;
        public Collider2D triggerColliderWall;

        public GameObject fireball;
        public GameObject mouth;

        private int selectedCodeLevel;
        private static readonly int AttackAnimHash = Animator.StringToHash("Attack");
        private static readonly int IsWalkingAnimHash = Animator.StringToHash("IsWalking");

        private bool isWalking = true;
        
        private void Start()
        {
            rb = GetComponent<Rigidbody2D>();
            anim = GetComponent<Animator>();
            selectedCodeLevel = PlayerDataManager.Data.unlockedCodingLevel;

            if (selectedCodeLevel == 0) return;
            switch (selectedCodeLevel)
            {
                case 2:
                    Flip();
                    StartCoroutine(Move());
                    break;
                case 3:
                    Flip();
                    anim.SetTrigger(AttackAnimHash);
                    StartCoroutine(Move());
                    break;
                /*case 4 :
                case 5 :
                case 6 :
                    StartCoroutine(MeleeAttackLoop());
                    break;*/
                
            }
        }

        private IEnumerator WaitBeforeWalking()
        {
            yield return new WaitForSeconds(2);
            isWalking = true;
            yield return null;
        }

        
        public void StartMeleeAttack()
        {
            attackHitbox.enabled = true;
        }

        public void EndMeleeAttack()
        {
            attackHitbox.enabled = false;
        }

        private void FixedUpdate()
        {
            if (selectedCodeLevel == 0) return;
            switch (selectedCodeLevel)
            {
                case 1:
                case 2:
                case 3: // Flip on enemy collision
                    if(triggerColliderWall.IsTouchingLayers(enemy))
                        Flip();
                    break;
                case 4 : // Move AND flip on enemy collision
                    if(triggerColliderWall.IsTouchingLayers(enemy))
                        Flip();
                    anim.SetBool(IsWalkingAnimHash, true);
                    rb.velocity = new Vector2(moveSpeed, rb.velocity.y);
                    break;
                case 5 : // Move, flip on enemy collision OR flip on ground edge
                    if (!triggerColliderGround.IsTouchingLayers(ground) || triggerColliderWall.IsTouchingLayers(enemy))
                        Flip();
                    anim.SetBool(IsWalkingAnimHash, true);
                    rb.velocity = new Vector2(moveSpeed, rb.velocity.y);
                    break;
                case 6 : // Move AND attack, flip on enemy collision OR flip on ground edge
                    if (!triggerColliderGround.IsTouchingLayers(ground) || triggerColliderWall.IsTouchingLayers(enemy))
                        Flip();
                    if(isWalking)
                    {
                        anim.SetBool(IsWalkingAnimHash, true);
                        rb.velocity = new Vector2(moveSpeed, rb.velocity.y);
                    }
                    Vector2 origin = transform.position + Vector3.up / 2;
                    Vector2 direction = Vector2.left * moveSpeed;
#if UNITY_EDITOR
                    Debug.DrawRay(origin, -direction * playerDetectionDistance);
#endif
                    
                    if (isWalking)
                    {
                        RaycastHit2D[] allHits = Physics2D.RaycastAll(origin, -direction, playerDetectionDistance);
                        foreach (var hit in allHits)
                        {
                            if(hit.collider != null && hit.collider.CompareTag("Player"))
                            {
                                isWalking = false;
                                StartCoroutine(WaitBeforeWalking());
                                anim.SetBool(IsWalkingAnimHash, false);
                                anim.SetTrigger(AttackAnimHash);
                            }
                        }
                    }
                    break;
                
            }
        }

        private void Flip()
        {
            Transform t = transform;
            Vector3 scale = t.localScale;
            t.localScale = new Vector2(scale.x * -1, scale.y);
            moveSpeed *= -1;
        }

        public void RangedAttack()
        {
            GameObject obj = Instantiate(fireball, mouth.transform.position, Quaternion.identity);
            obj.GetComponent<Fireball>().speed *= -moveSpeed;
        }

        private IEnumerator MeleeAttackLoop()
        {
            yield return new WaitForSeconds(2f);
            anim.SetTrigger(AttackAnimHash); 
            StartCoroutine(MeleeAttackLoop());
            yield return null;
        }

        private IEnumerator Move()
        {
            float elapsedTime = 0f;
            anim.SetBool(IsWalkingAnimHash, true);
            while (elapsedTime < 5)
            {
                rb.velocity = new Vector2(moveSpeed, rb.velocity.y);
                elapsedTime += Time.deltaTime;
                yield return null;
            }
            anim.SetBool(IsWalkingAnimHash, false);
            yield return null;
        }
    }
}
