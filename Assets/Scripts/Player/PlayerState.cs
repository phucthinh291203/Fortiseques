using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class PlayerState
{

    protected PlayerStateMachine stateMachine;
    protected Player player;
    protected Rigidbody2D rb;


    private string animBoolName;
    protected float xinput;
    protected float yinput;
    protected float stateTimer;
    protected bool triggerCalled;
    public PlayerState(Player _player,PlayerStateMachine _stateMachine,string _animBoolName)
    {
        this.player = _player;
        this.stateMachine = _stateMachine;
        this.animBoolName = _animBoolName;
    }

    public virtual void Enter()
    {
        player.anim.SetBool(animBoolName, true);
        rb = player.rb;
        triggerCalled = false;
    }

    public virtual void Update()
    {
        if (Time.timeScale == 0)    
            return;

        xinput = player.joystick.Horizontal;
        yinput = player.joystick.Vertical;
        player.anim.SetFloat("yVelocity", rb.velocity.y);
        stateTimer -= Time.deltaTime;
    }

    public virtual void Exit()
    {
        player.anim.SetBool(animBoolName, false);
    }

    public virtual void AnimationFinishTrigger()
    {
        triggerCalled = true;
    }
}
