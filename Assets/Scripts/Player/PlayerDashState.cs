using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerDashState : PlayerState
{
    public PlayerDashState(Player _player, PlayerStateMachine _stateMachine, string _animBoolName) : base(_player, _stateMachine, _animBoolName)
    {
    }

    public override void Enter()
    {
        base.Enter();

        player.skill.dash.CloneOnDash();

        stateTimer = player.dashDuration;
        player.stats.SetInvicible(true);
    }

    public override void Exit()
    {
        base.Exit();
        player.skill.dash.CloneOnArrivals();

        player.SetVelocity(0, rb.velocity.y);
        player.stats.SetInvicible(false);
    }

    public override void Update()
    {
        base.Update();
        player.SetVelocity(player.dashDirection * player.dashSpeed, 0);
        
        if (!player.IsGroundDetect() && player.IsWallDetect())
            stateMachine.ChangeState(player.wallSlideState);

        if(stateTimer < 0) {
            stateMachine.ChangeState(player.idleState);
        }

        player.fx.CreateAfterImage();
    }
}
