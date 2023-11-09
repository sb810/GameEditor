using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace LevelEditor.InGame
{
    public class Player : MonoBehaviour
    {
        public float movingSpeed;
        public float jumpForce;
        public float invincibilityTime;
        public float loseControlAfterHit;
        public int maxHp;


        private List<GameObject> heart;

        [HideInInspector] public int hp;
        [HideInInspector] public bool isInvincible;
        [HideInInspector]  public Vector3 lastCheckpoint;

        private float moveInput;
        private bool canMove = true;
        private bool facingRight = true;
        private float facing = 1f;
        private bool isGrounded;


        [SerializeField] private Transform groundCheck;
        [SerializeField] private Animator dustParticlesAnimator;

        private Rigidbody2D rb;

        [HideInInspector] public int score;
        [SerializeField] private TextMeshProUGUI scoreLabel;
        [SerializeField] private GameObject heartContainer;

        private GameOverManager gameOverManager;
        private Animator anim;

        public GameObject feet;
        private static readonly int IsJumping = Animator.StringToHash("isJumping");
        private static readonly int IsMoving = Animator.StringToHash("isMoving");
        private static readonly int Active = Animator.StringToHash("active");

        private bool willDie;
        private bool dying;

        private void Start()
        {
            gameOverManager = GameManager.Instance.GameOverManager;
            rb = GetComponent<Rigidbody2D>();
            anim = GetComponent<Animator>();
            lastCheckpoint = transform.position;
            heart = new List<GameObject>();
        
            for (int i = 0; i < heartContainer.transform.childCount; i++)
                heart.Add(heartContainer.transform.GetChild(i).gameObject);
        
            hp = maxHp;
            UpdateLife();
            UpdateScore();
        }

        private void Update()
        {
            /*if (willDie)
            {
                if (!isGrounded) return;
                willDie = false;
                dying = true;
                return;
            }

            if (dying)
            {
                
                return;
            }*/
            
            var pos = transform.position;
            RaycastHit2D hit = Physics2D.Raycast(pos + new Vector3(0.35f*facing, 0, 0), Vector2.up, 1);
        
            if (hit.collider != null)
                canMove = !hit.collider.CompareTag("Wall");
            else
                canMove = true;

            if (Input.GetButton("Horizontal")&&canMove)
            {
                moveInput = Input.GetAxisRaw("Horizontal");
                Vector3 direction = transform.right * moveInput;
                transform.position = Vector3.MoveTowards(pos, pos + direction, movingSpeed * Time.deltaTime);
            } 
            
            if (Input.GetButtonDown("Jump") && isGrounded)
            {
                rb.AddForce(transform.up * jumpForce, ForceMode2D.Impulse);
            }

            switch (facingRight)
            {
                case false when moveInput > 0:
                case true when moveInput < 0:
                    Flip();
                    break;
            }
        
            if(hp == 0 || transform.position.y <= -13)
            {
                //willDie = true;
                Death();
            }
        }

        private void FixedUpdate()
        {
            CheckGround();

            anim.SetBool(IsJumping, !isGrounded);

            moveInput = Input.GetAxisRaw("Horizontal");
            if (moveInput != 0)
            {
                anim.SetBool(IsMoving, true);
                if (dustParticlesAnimator == null) return;
                if (isGrounded)
                {
                    dustParticlesAnimator.SetBool(Active, true);
                    dustParticlesAnimator.Play("Run_dust", 0, anim.GetCurrentAnimatorStateInfo(0).normalizedTime);
                }
                else dustParticlesAnimator.SetBool(Active, false);
            }
            else
            {
                anim.SetBool(IsMoving, false);
                if (dustParticlesAnimator != null) dustParticlesAnimator.SetBool(Active, false); 
            }

            if (rb.velocity.x > 10)
                rb.velocity = new Vector2(10,rb.velocity.y);
            if (rb.velocity.y > 10)
                rb.velocity = new Vector2(rb.velocity.x,10);

            if (rb.velocity.x < -10)
                rb.velocity = new Vector2(-10, rb.velocity.y);
            if (rb.velocity.y < -10)
                rb.velocity = new Vector2(rb.velocity.x, -10);
        }

        private void Flip()
        {
            GameObject go = anim.gameObject;
            var localScale = go.transform.localScale;
            facingRight = !facingRight;
            facing *= -1;
            go.transform.localScale =new Vector3(-localScale.x, localScale.y, localScale.z);
        }

        private void CheckGround()
        {
            Collider2D[] colliders = Physics2D.OverlapCircleAll(groundCheck.transform.position, 0.2f);
            isGrounded = colliders.Length > 2;
        }

        public void UpdateScore()
        {
            if(scoreLabel)
                scoreLabel.text = score.ToString();
        }
    
        public void UpdateLife()
        {
            Debug.Log(heart);
            Debug.Log(hp);
        
            foreach (GameObject obj in heart)
                obj.SetActive(hp >= heart.IndexOf(obj) + 1);
        }

        public IEnumerator InvincibleTimer()
        {
            isInvincible = true;
            GetComponent<SpriteRenderer>().color = new Color(1, 1, 1, 0.5f);
            canMove = false;
            feet.SetActive(false);
            yield return new WaitForSeconds(loseControlAfterHit);
            canMove = true;
            feet.SetActive(true);
            yield return new WaitForSeconds(invincibilityTime-loseControlAfterHit);
            GetComponent<SpriteRenderer>().color = new Color(1, 1, 1, 1);
            isInvincible = false;
            yield return null;
        }

        public IEnumerator InvincibleTimer(float time)
        {
            isInvincible = true;
            //GetComponent<Renderer>().color = new Color(1, 1, 1, 0.2f);
            canMove = false;
            feet.SetActive(false);
            yield return new WaitForSeconds(loseControlAfterHit);
            canMove = true;
            feet.SetActive(true);
            yield return new WaitForSeconds(time - loseControlAfterHit);
            //GetComponent<Renderer>().color = new Color(1, 1, 1, 1);
            isInvincible = false;
            yield return null;
        }

        private void Death()
        {
            hp = maxHp;
            //GetComponent<Renderer>().color = new Color(1, 1, 1, 1);
            isInvincible = false;
            canMove = true;
            feet.SetActive(true);
            StopAllCoroutines();
            UpdateLife();
        
            gameOverManager.gameOverMenu.SetActive(true);
            gameOverManager.inGameUI.SetActive(false);
            gameOverManager.player = gameObject;    

            //anim.SetBool("");
            gameObject.SetActive(false);

        }

        public void TpToLastCheckpoint()
        {
            transform.position = lastCheckpoint;
        }

        public void Bounce(Vector2 force)
        {
            rb.velocity = Vector2.zero;
            rb.AddForce(force , ForceMode2D.Impulse );
        }
    }
}
