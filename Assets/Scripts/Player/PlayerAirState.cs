using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAirState : PlayerState
{
    public PlayerAirState(Player _player, PlayerStateMachine _stateMachine, string _animBoolName) : base(_player, _stateMachine, _animBoolName)
    {
    }

    public override void Enter()
    {
        base.Enter();
    }

    public override void Exit()
    {
        base.Exit();
    }

    public override void Update()
    {
        base.Update();
        if (player.IsWallDetect())
            stateMachine.ChangeState(player.wallSlideState);
        if(player.IsGroundDetect())
            stateMachine.ChangeState(player.idleState);
        if (xinput != 0)
            player.SetVelocity(player.moveSpeed * .8f * xinput, rb.velocity.y);
    }
}
