using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;
using UnityEngine.UI;

public class Player : Entity
{
    public FixedJoystick joystick;

    

    [Header("Attack details")]
    public Vector2[] attackMove;
    public float counterAttackDuration = 0.2f;

    public bool isBusy {  get; private set; }
    [Header("Move info")]
    public float moveSpeed = 12f;
    public float jumpForce;
    public float swordReturnImpact;
    private float defaultMoveSpeed;
    private float defaultJumpForce;


    [Header("Dash info")]
    public float dashSpeed;
    public float dashDuration;
    private float defaultDashSpeed;
    public float dashDirection { get; private set; }

    public float time;
    public float cooldown = 5f;

    public SkillManager skill;
    public GameObject sword;
    
    #region State
    public PlayerStateMachine stateMachine {  get; private set; }
    public PlayerIdleState idleState { get; private set; }
    public PlayerMoveState moveState { get; private set; }
    public PlayerAirState airState { get; private set; }
    public PlayerJumpState jumpState { get; private set; }
    public PlayerDashState dashState { get; private set; }
    public PlayerWallSlideState wallSlideState { get; private set; }
    public PlayerWallJumpState wallJumpState { get; private set; }
    public PlayerPrimaryAttackState primaryAttackState { get; private set; }
    public PlayerCounterAttackState counterAttackState { get; private set; }
    public PlayerAimSwordState aimSwordState { get; private set; }
    public PlayerCatchSwordState catchSwordState { get; private set; }
    public PlayerBlackHoleState blackHoleState { get; private set; }

    public PlayerDeathState deadthState {  get; private set; }
    #endregion
    protected override void Awake()
    {
        base.Awake();
        stateMachine = new PlayerStateMachine();
        idleState = new PlayerIdleState(this, stateMachine, "Idle");
        moveState = new PlayerMoveState(this, stateMachine, "Move");
        jumpState = new PlayerJumpState(this, stateMachine, "Jump");
        airState  = new PlayerAirState(this, stateMachine, "Jump");
        dashState = new PlayerDashState(this, stateMachine, "Dash");
        wallSlideState = new PlayerWallSlideState(this, stateMachine, "WallSlide");
        wallJumpState = new PlayerWallJumpState(this, stateMachine, "Jump");
        primaryAttackState = new PlayerPrimaryAttackState(this, stateMachine, "Attack");
        counterAttackState = new PlayerCounterAttackState(this, stateMachine, "CounterAttack");
        aimSwordState = new PlayerAimSwordState(this, stateMachine, "AimSword");
        catchSwordState = new PlayerCatchSwordState(this, stateMachine, "CatchSword");
        blackHoleState = new PlayerBlackHoleState(this, stateMachine, "Jump");
        deadthState = new PlayerDeathState(this, stateMachine, "Die");
    }

    protected override void Start()
    {
        base.Start();

        skill = SkillManager.instance;
        stateMachine.Initialize(idleState);
        defaultMoveSpeed = moveSpeed;
        defaultJumpForce = jumpForce;
        defaultDashSpeed = dashSpeed;
    }
        
    protected override void Update()
    {
        base.Update();
        stateMachine.currentState.Update();
        CheckForDashInput();

        if (Time.timeScale == 0)
            return;

        //if (/*PlayerInputManager.instance.isCrystal*/ Input.GetKey(KeyCode.F) && skill.crystal.crystalUnlocked)
        //{
        //    skill.crystal.CanUseSkill();
        //}
       
        if (PlayerInputManager.instance.isUseFlask)
        {
            Inventory.instance.UseFlask();
        }
    }

    public void UseCrystal()
    {
        if(skill.crystal.crystalUnlocked)
            skill.crystal.CanUseSkill();
    }


    public override void SlowEntityBy(float _slowPercentage, float _slowDuration)
    {
        moveSpeed = moveSpeed * (1 - _slowPercentage);
        jumpForce = jumpForce * (1 - _slowPercentage);
        dashSpeed = dashSpeed * (1 - _slowPercentage);
        anim.speed = anim.speed * (1- _slowPercentage);

        Invoke("ReturnDefaultSpeed", _slowDuration);
    }

    protected override void ReturnDefaultSpeed()
    {
        base.ReturnDefaultSpeed();
        moveSpeed = defaultMoveSpeed;
        jumpForce = defaultJumpForce;
        dashSpeed = defaultDashSpeed;

    }

    public IEnumerator BusyFor(float _time)
    {
        isBusy = true;
        yield return new WaitForSeconds(_time);
        isBusy = false;
    }

    public void AnimationTrigger()=> stateMachine.currentState.AnimationFinishTrigger();

    public void CheckForDashInput()
    {
        

        if(IsWallDetect())
        {
            return;
        }
        
        if (skill.dash.dashUnlocked == false)
            return;
        
        if (/*Input.GetKeyDown(KeyCode.LeftShift)*/PlayerInputManager.instance.isDash && SkillManager.instance.dash.CanUseSkill()) 
        {
            Debug.Log("dash");
            dashDirection = Input.GetAxisRaw("Horizontal");

            if (dashDirection == 0)
                dashDirection = facingDirection;

            
            stateMachine.ChangeState(dashState);
        }
    }

    public void AssignNewSword(GameObject _newSword)
    {
        sword = _newSword;
    }

    public void CatchTheSword()
    {
        stateMachine.ChangeState(catchSwordState);
        Destroy(sword);
    }

    public override void Die()
    {
        base.Die();
        stateMachine.ChangeState(deadthState);
    }

    protected override void SetZeroKnockBackPower()
    {
        knockBackPower = new Vector2(0, 0);
    }

}
