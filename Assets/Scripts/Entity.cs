using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Entity : MonoBehaviour
{
    public Animator anim { get; private set; }
    public Rigidbody2D rb { get; private set; }
    public EntityFx fx { get; private set; }
    public CharacterStats stats { get; private set; }
    public SpriteRenderer sr { get; private set; }
    public CapsuleCollider2D cd { get; private set; }

    [Header("Knock Back info")]
    [SerializeField] protected Vector2 knockBackPower;
    [SerializeField] protected float knockBackDuration;
    protected bool isKnocked;

    [Header("Collision Info")]
    public Transform attackCheck;
    public float attackCheckRadius;
    [SerializeField] protected Transform wallCheck;
    [SerializeField] protected float wallCheckDistance;
    [SerializeField] protected Transform groundCheck;
    [SerializeField] protected float groundCheckDistance;
    [SerializeField] protected LayerMask whatIsGround;

    public Action onFlipped;


    public int knockBackDir { get;private set; }
    public float facingDirection { get; private set; } = 1;
    protected bool facingRight = true;
    protected virtual void Awake()
    {

    }

    protected virtual void Start()
    {
        fx = GetComponent<EntityFx>();
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponentInChildren<Animator>();
        sr = GetComponentInChildren<SpriteRenderer>();
        stats = GetComponent<CharacterStats>();
        cd = GetComponent<CapsuleCollider2D>();
    }
    protected virtual void Update()
    {
        
    }

    public virtual void SlowEntityBy(float _slowPercentage,float _slowDuration)
    {
       
    }

    protected virtual void ReturnDefaultSpeed()
    {
        anim.speed = 1;
    }

    public virtual void DamageImpact()
    {
        StartCoroutine("HitKnockBack");
    }

    public virtual void SetupKnockBackDir(Transform _damageDirection)
    {
        if(_damageDirection.position.x > transform.position.x)
        {
            knockBackDir = -1;
        }

        else if (_damageDirection.position.x < transform.position.x)
        {
            knockBackDir = 1;
        }
    }
    public virtual void Die()
    {

    }

    public void SetUpKnockBackPower(Vector2 _knockBackPower)
    {
        knockBackPower = _knockBackPower;
    }

    protected virtual IEnumerator HitKnockBack()
    {
        isKnocked = true;
        rb.velocity = new Vector2(knockBackPower.x * knockBackDir, knockBackPower.y);
        yield return new WaitForSeconds(knockBackDuration);
        isKnocked = false;
        SetZeroKnockBackPower();
    }

    #region Collision Detecting
    public virtual bool IsGroundDetect() => Physics2D.Raycast(groundCheck.position, Vector2.down, groundCheckDistance, whatIsGround);
    public virtual bool IsWallDetect() => Physics2D.Raycast(wallCheck.position, Vector2.right * facingDirection, wallCheckDistance, whatIsGround);
    protected virtual void OnDrawGizmos()
    {
        Gizmos.DrawLine(groundCheck.position, new Vector3(groundCheck.position.x, groundCheck.position.y - groundCheckDistance));
        Gizmos.DrawLine(wallCheck.position, new Vector3(wallCheck.position.x + wallCheckDistance * facingDirection, wallCheck.position.y));
        Gizmos.DrawWireSphere(attackCheck.position, attackCheckRadius);
    }
    #endregion

    #region flip
    public virtual void Flip()
    {
        facingDirection = facingDirection * -1;
        facingRight = !facingRight;
        transform.Rotate(0, 180, 0);

        //if (onFlipped != null)
        //    onFlipped();

        onFlipped?.Invoke();
    }

    public virtual void FlipController(float _x)
    {
        if (_x > 0 && !facingRight)
            Flip();
        else if (_x < 0 && facingRight)
            Flip();
    }
    #endregion


    #region Velocity Changes
    public virtual void SetVelocity(float xVelocity, float yVelocity)
    {
        if (isKnocked)
            return;
        rb.velocity = new Vector2(xVelocity, yVelocity);
        FlipController(xVelocity);
    }

    public virtual void SetZeroVelocity() 
    {
        if (isKnocked) 
            return;
        rb.velocity = new Vector2(0, 0);
    }

    protected virtual void SetZeroKnockBackPower()
    {

    }
    #endregion
}
