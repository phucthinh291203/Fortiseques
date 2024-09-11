using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMoveState : PlayerGroundState
{
    public PlayerMoveState(Player _player, PlayerStateMachine _stateMachine, string _animBoolName) : base(_player, _stateMachine, _animBoolName)
    {
    }

    public override void Enter()
    {
        base.Enter();
        AudioManager.instance.PlaySFX(13, null);
    }

    public override void Exit()
    {
        base.Exit();
        AudioManager.instance.StopSFX(13);
    }

    public override void Update()
    {
        base.Update();
        player.SetVelocity(xinput * player.moveSpeed, rb.velocity.y);

        if (Time.timeScale == 0)
            AudioManager.instance.StopSFX(13);

        if (xinput == 0/*|| player.IsWallDetect()*/)
            stateMachine.ChangeState(player.idleState);
    }
}
