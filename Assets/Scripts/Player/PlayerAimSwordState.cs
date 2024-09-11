using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAimSwordState : PlayerState
{
    public PlayerAimSwordState(Player _player, PlayerStateMachine _stateMachine, string _animBoolName) : base(_player, _stateMachine, _animBoolName)
    {
    }

    public override void Enter()
    {
        base.Enter();
        player.skill.sword.DotsActive(true);
    }

    public override void Exit()
    {
        base.Exit();
        player.StartCoroutine("BusyFor", 0.2f);
    }

    public override void Update()
    {
        base.Update();
        player.SetZeroVelocity();

        if (PlayerInputManager.instance.isAim == false)
            stateMachine.ChangeState(player.idleState);

        ////Vector2 mouseDirection = new Vector2(player.transform.position.x * player.facingDirection, 2);
        //if(/*player.transform.position.x > mouseDirection.x && */player.facingDirection == 1)
        //{
        //    player.Flip();
        //}
        //else if(/*player.transform.position.x < mouseDirection.x &&*/ player.facingDirection == -1)
        //{
        //    player.Flip();
        //}
    }
}
