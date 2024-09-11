using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkeletonStunState : EnemyState
{
    Enemy_Skeleton enemy;

    public SkeletonStunState(Enemy _enemyBase, EnemyStateMachine _stateMachine, string _anim,Enemy_Skeleton _enemy) : base(_enemyBase, _stateMachine, _anim)
    {
        this.enemy = _enemy;
    }

    public override void Enter()
    {
        base.Enter();
        stateTimer = enemy.stunDuration;
        enemy.fx.InvokeRepeating("RedColorBlink", 0, 0.1f);
        rb.velocity = new Vector2(-enemy.facingDirection * enemy.stunDirection.x, enemy.stunDirection.y);
    }

    public override void Exit()
    {
        base.Exit();
        enemy.fx.Invoke("CancelColorChange", 0);
    }

    public override void Update()
    {
        base.Update();
        if (stateTimer < 0)
        {
            
            stateMachine.ChangeState(enemy.idleState);
        }
    }
}
