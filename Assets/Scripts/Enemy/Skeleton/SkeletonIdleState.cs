using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkeletonIdleState : SkeletonGroundState
{
    public SkeletonIdleState(Enemy _enemyBase, EnemyStateMachine _stateMachine, string _anim, Enemy_Skeleton _enemy) : base(_enemyBase, _stateMachine, _anim, _enemy)
    {

    }

    public override void Enter()
    {
        base.Enter();
        stateTimer = enemy.idleTime;
    }

    public override void Exit()
    {
        base.Exit();
        AudioManager.instance.PlaySFX(22,enemy.transform);
    }

    public override void Update()
    {
        base.Update();
        if(stateTimer < 0) {
            stateMachine.ChangeState(enemy.moveState);
        }
    }
}
