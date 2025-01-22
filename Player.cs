using Cinemachine;
using System.Collections;
using Unity.VisualScripting;
using UnityEngine;

public class Player : MonoBehaviour
{
    [Header("Movement Settings")]
    public float speed;
    private Rigidbody2D rb;
    private bool facingRight = true;


    [Header("Ground Settings")]
    private bool isGrounded;
    public Transform groundCheck;
    public float checkDistance = 0.5f;
    public LayerMask whatIsGround;
    public float jumpForce;


    [Header("Slide and Jump Settings")]
    private bool isTouchingFront;
    public Transform frontCheck;
    private bool wallJumping;
    public float wallJumpTime;
    public float xWallForce;
    public float yWallForce;
    private bool wallSliding;
    public float wallSlidingSpeed;


    [Header("Powers and Abilities")]
    private bool canDash = true;
    private bool isDashing;
    private float dashingPower = 25f;
    private float dashingTime = 0.2f;
    private bool canHack = true;
    private bool isHacking;
    private float hackingPower = 7f;
    private float hackingTime = 1f;
    

    [Header("Timers")]
    private float coyoteTimer;
    private float jumpBufferTimer;


    [Header("Animation")]
    private Animator anim;


    [Header("Health")]
    public int health;
    public LayerMask enemyLayer;


    [Header("LightAttacks")]
    public float timeBetweenAttacks;
    private float nextAttackTime;
    public Transform attackPoint;
    public float attackRange;
    public int damage;


    [Header("LightAttacks")]
    public float timeBetweenHeavyAttacks;
    private float nextHeavyAttackTime;
    public Transform HeavyAttackPoint;
    public float HeavyAttackRange;
    public int heavyDamage;




    [Header("Miscellaneous")]
    public float hitPauseDuration = 0.01f;
    private bool isAttacking = false;
    private bool useAttackA = true;
    private bool isDead = false;
    [SerializeField] private TrailRenderer tr;
    [SerializeField] private float hackingCooldown;
    [SerializeField] private int hackDamage;
    [SerializeField] private float dashingCooldown;




    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
    }

    private void Update()
    {
        if (!isDead)
        {
            HandleMovement();
        }

        HandleAnimations();

        if (!isDead)
        {
            HandleAttack();
        }

        if (!wallSliding && Time.time > nextHeavyAttackTime)
        {
            if (Input.GetKey(KeyCode.O))
            {
                anim.SetTrigger("hattack");
                nextHeavyAttackTime = Time.time + timeBetweenHeavyAttacks;
            }
        }
    }

    private void HandleAttack()
    {
        if (Time.time >= nextAttackTime)
        {
            if (Input.GetKeyDown(KeyCode.I) && !isAttacking)
            {
                PerformAttack();
            }
        }
        else
        {
            isAttacking = false;
        }
    }

    private void PerformAttack()
    {
        Collider2D[] enemiesToDamage = Physics2D.OverlapCircleAll(attackPoint.position, attackRange, enemyLayer);
        foreach (Collider2D col in enemiesToDamage)
        {
            col.GetComponent<Enemy>().TakeDamage(damage);
        }

        if (useAttackA)
        {
            anim.SetTrigger("attacka");
        }
        else
        {
            anim.SetTrigger("attackb");
        }

        useAttackA = !useAttackA;
        nextAttackTime = Time.time + timeBetweenAttacks;
        isAttacking = true;
    }

    private void HandleMovement()
    {
        if (isDashing || isHacking)
        {
            return;
        }

        float input = Input.GetAxisRaw("Horizontal");
        Vector2 moveDirection = new Vector2(input, 0) * speed;

        rb.velocity = new Vector2(moveDirection.x, rb.velocity.y);

        if (input > 0 && !facingRight)
        {
            Flip();
        }
        else if (input < 0 && facingRight)
        {
            Flip();
        }

        CheckGround();

        if (isGrounded)
        {
            coyoteTimer = 0.1f;
        }
        else
        {
            coyoteTimer -= Time.deltaTime;
        }

        if (Input.GetKeyDown(KeyCode.Space) && (isGrounded || coyoteTimer > 0 || jumpBufferTimer > 0))
        {
            rb.velocity = new Vector2(rb.velocity.x, jumpForce);
            jumpBufferTimer = 0;
            anim.SetBool("isJumping", true);
        }
        else if (isGrounded && rb.velocity.y <= 0)
        {
            anim.SetBool("isJumping", false);
        }

        CheckWallSliding(input);

        if (Input.GetKeyDown(KeyCode.Space) && wallSliding)
        {
            wallJumping = true;
            Invoke("SetWallJumpingToFalse", wallJumpTime);
            rb.velocity = new Vector2(xWallForce * -input, yWallForce);
        }

        if (wallJumping)
        {
            rb.velocity = new Vector2(xWallForce * -input, yWallForce);
        }

        if (Input.GetKeyDown(KeyCode.Space))
        {
            jumpBufferTimer = 0.1f;
        }
        else if (jumpBufferTimer > 0)
        {
            jumpBufferTimer -= Time.deltaTime;
        }

        if (Input.GetKeyDown(KeyCode.LeftShift) && canDash)
        {
            StartCoroutine(Dash());
        }

        if (Input.GetKeyDown(KeyCode.P) && canHack && isGrounded)
        {
            StartCoroutine(Hack());
        }
    }

    private void CheckGround()
    {
        RaycastHit2D hit = Physics2D.Raycast(groundCheck.position, Vector2.down, checkDistance, whatIsGround);
        isGrounded = hit.collider != null;
    }

    private void CheckWallSliding(float input)
    {
        isTouchingFront = Physics2D.OverlapCircle(frontCheck.position, 0.1f, whatIsGround);


        if (isTouchingFront && !isGrounded && rb.velocity.y < 0)
        {
            wallSliding = true;
            anim.SetBool("isWallSliding", true);
            rb.velocity = new Vector2(rb.velocity.x, Mathf.Clamp(rb.velocity.y, -wallSlidingSpeed, float.MaxValue));
        }
        else
        {
            wallSliding = false;
            anim.SetBool("isWallSliding", false);
        }
    }


    private void HandleAnimations()
    {
        float input = Input.GetAxisRaw("Horizontal");
        if (input != 0)
        {
            anim.SetBool("isRunning", true);
        }
        else
        {
            anim.SetBool("isRunning", false);
        }
    }

    private void Flip()
    {
        facingRight = !facingRight;
        Vector3 scaler = transform.localScale;
        scaler.x *= -1;
        transform.localScale = scaler;
    }

    private void SetWallJumpingToFalse()
    {
        wallJumping = false;
    }

    private IEnumerator Dash()
    {
        canDash = false;
        isDashing = true;
        anim.SetBool("isDashing", true);
        float originalGravity = rb.gravityScale;
        rb.gravityScale = 0f;
        rb.velocity = new Vector2(transform.localScale.x * dashingPower, 0f);
        tr.emitting = true;

        Collider2D playerCollider = GetComponent<Collider2D>();
        GameObject[] enemies = GameObject.FindGameObjectsWithTag("Enemy");
        Collider2D[] enemyColliders = new Collider2D[enemies.Length];
        for (int i = 0; i < enemies.Length; i++)
        {
            enemyColliders[i] = enemies[i].GetComponent<Collider2D>();
            if (enemyColliders[i] != null)
            {
                Physics2D.IgnoreCollision(playerCollider, enemyColliders[i], true);
            }
        }

        yield return new WaitForSeconds(dashingTime);
        tr.emitting = false;
        rb.gravityScale = originalGravity;
        isDashing = false;
        anim.SetBool("isDashing", false);

        for (int i = 0; i < enemyColliders.Length; i++)
        {
            if (enemyColliders[i] != null)
            {
                Physics2D.IgnoreCollision(playerCollider, enemyColliders[i], false);
            }
        }

        yield return new WaitForSeconds(dashingCooldown);
        canDash = true;
    }

    private IEnumerator Hack()
    {
        canHack = false;
        isHacking = true;
        anim.SetBool("isHacking", true);
        float originalGravity = rb.gravityScale;
        rb.gravityScale = 0f;
        rb.velocity = new Vector2(transform.localScale.x * hackingPower, 0f);

        int damageDealt = hackDamage;

        yield return new WaitForSeconds(hackingTime);

        rb.gravityScale = originalGravity;
        isHacking = false;
        anim.SetBool("isHacking", false);

        if (damageDealt > 0)
        {
            foreach (Collider2D enemyCollider in Physics2D.OverlapCircleAll(transform.position, attackRange, enemyLayer))
            {
                Enemy enemy = enemyCollider.GetComponent<Enemy>();
                if (enemy != null)
                {
                    enemy.TakeDamage(damageDealt);
                }
            }
        }

        yield return new WaitForSeconds(hackingCooldown);
        canHack = true;
    }

    void OnTriggerStay2D(Collider2D other)
    {
        if (isHacking && other.CompareTag("Enemy"))
        {
            Enemy enemy = other.GetComponent<Enemy>();
            if (enemy != null)
            {
                enemy.TakeDamage(hackDamage);
            }
        }
    }

    public void TakeDamage(int damage)
    {
        health -= damage;
        if (health <= 0)
        {
            Die();
        }
    }

    private void Die()
    {
        isDead = true;
        rb.velocity = Vector2.zero;
        anim.SetTrigger("death");
        StartCoroutine(WaitForAnimationAndDestroy());
    }

    private IEnumerator WaitForAnimationAndDestroy()
    {
        AnimatorStateInfo stateInfo = anim.GetCurrentAnimatorStateInfo(0);
        float animationLength = stateInfo.length;
        yield return new WaitForSeconds(animationLength);
        float additionalDelay = 1.0f;
        yield return new WaitForSeconds(additionalDelay);
        Destroy(gameObject);
    }

    public void Attack()
    {
        Collider2D[] enemiesToDamage = Physics2D.OverlapCircleAll(attackPoint.position, attackRange, enemyLayer);
        foreach (Collider2D col in enemiesToDamage)
        {
            col.GetComponent<Enemy>().TakeDamage(damage);
        }
    }

    public void heavyAttack()
    {
        Collider2D[] enemiesToDamage = Physics2D.OverlapCircleAll(HeavyAttackPoint.position, HeavyAttackRange, enemyLayer);
        foreach (Collider2D col in enemiesToDamage)
        {
            Enemy enemy = col.GetComponent<Enemy>();
            if (enemy != null)
            {
                enemy.TakeDamage(heavyDamage);
                StartCoroutine(HitPauseCoroutine());
            }
        }
    }

    private IEnumerator HitPauseCoroutine()
    {
        Time.timeScale = 0.01f;
        yield return new WaitForSecondsRealtime(hitPauseDuration);
        Time.timeScale = 1f;
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(attackPoint.position, attackRange);
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(HeavyAttackPoint.position, HeavyAttackRange);
    }
}
