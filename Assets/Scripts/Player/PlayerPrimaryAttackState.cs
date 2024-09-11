using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerPrimaryAttackState : PlayerState
{
    public int comboCounter;
    private float lastTimeAttacked;
    private float comboWindow = 2;
   
    public PlayerPrimaryAttackState(Player _player, PlayerStateMachine _stateMachine, string _animBoolName) : base(_player, _stateMachine, _animBoolName)
    {

    }

    public override void Enter()
    {
        base.Enter();
        //AudioManager.instance.PlaySFX(1);
        xinput = 0; // can cai nay de fix bug attack sai huong (thinh thoang xay ra)
        
        if (comboCounter > 2 || Time.time >= lastTimeAttacked + comboWindow)
            comboCounter = 0;

        float attackDirection = player.facingDirection;
        if (xinput != 0)
            attackDirection = xinput;

        player.SetVelocity(player.attackMove[comboCounter].x * attackDirection, player.attackMove[comboCounter].y);
        player.anim.SetInteger("ComboCounter", comboCounter);
        
        stateTimer = .1f;
    }

    public override void Exit()
    {
        base.Exit();
        player.StartCoroutine("BusyFor", 0.15f);
        comboCounter++;
        lastTimeAttacked = Time.time;
       
    }

    public override void Update()
    {
        base.Update();
        if (stateTimer < 0)
            player.SetZeroVelocity();

        if (triggerCalled)
        {
            stateMachine.ChangeState(player.idleState);
        }
    }
}
