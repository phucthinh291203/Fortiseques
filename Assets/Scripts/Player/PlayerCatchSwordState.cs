using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class PlayerCatchSwordState : PlayerState
{
    private Transform sword;
    public PlayerCatchSwordState(Player _player, PlayerStateMachine _stateMachine, string _animBoolName) : base(_player, _stateMachine, _animBoolName)
    {
    }

    public override void Enter()
    {
        base.Enter();
        sword = player.sword.transform;
        if (player.transform.position.x > sword.position.x && player.facingDirection == 1)
        {
            player.Flip();
        }
        else if (player.transform.position.x < sword.position.x && player.facingDirection == -1)
        {
            player.Flip();
        }
        //rb.velocity = new Vector2(player.swordReturnImpact * -player.facingDirection,rb.velocity.y);
        player.fx.PlayDustFx();
        player.fx.ScreenShake(player.fx.shakeSwordCatchPower);
    }

    public override void Exit()
    {
        base.Exit();
        player.StartCoroutine("BusyFor", 0.15f);
    }

    public override void Update()
    {
        base.Update();
        if (triggerCalled)
        {
            stateMachine.ChangeState(player.idleState);
        }
    }
}
