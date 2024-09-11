using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkeletonMoveState : SkeletonGroundState
{
    

    public SkeletonMoveState(Enemy _enemyBase, EnemyStateMachine _stateMachine, string _anim, Enemy_Skeleton _enemy) : base(_enemyBase, _stateMachine, _anim, _enemy)
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
        enemy.SetVelocity(enemy.moveSpeed * enemy.facingDirection, rb.velocity.y);
        if (!enemy.IsGroundDetect() || enemy.IsWallDetect())
        {
            enemy.Flip();
            stateMachine.ChangeState(enemy.idleState);
        }
    }
}
