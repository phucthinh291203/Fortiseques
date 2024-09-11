using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices.WindowsRuntime;
using UnityEngine;

public class SkeletonBattleState : EnemyState
{
    private Transform player;
    private Enemy_Skeleton enemy;
    private int moveDirection;
    public SkeletonBattleState(Enemy _enemyBase, EnemyStateMachine _stateMachine, string _anim,Enemy_Skeleton _enemy) : base(_enemyBase, _stateMachine, _anim)
    {
        this.enemy = _enemy;
    }

    public override void Enter()
    {
        base.Enter();
        
        player = PlayerManager.instance.player.transform;
        if (player.GetComponent<PlayerStats>().isDead)
            stateMachine.ChangeState(enemy.moveState);
    }

    public override void Update()
    {
        base.Update();
        if (enemy.IsPlayerDetect())
        {
            stateTimer = enemy.battleTime;
            if(enemy.IsPlayerDetect().distance < enemy.attackDistance)
            {
                if(CanAttack())
                    stateMachine.ChangeState(enemy.attackState);
            }
        }
        else
        {
            if (stateTimer < 0 || Vector2.Distance(player.transform.position, enemy.transform.position)>7)
                stateMachine.ChangeState(enemy.idleState);
        }


        if (player.position.x > enemy.transform.position.x)
            moveDirection = 1;
        else if (player.position.x < enemy.transform.position.x)
            moveDirection = -1;

        enemy.SetVelocity(enemy.moveSpeed * moveDirection, rb.velocity.y);

        
    }
    public override void Exit()
    {
        base.Exit();
    }

    public bool CanAttack()
    {
        if (Time.time >= enemy.lastTimeAttack + enemy.acttackCoolDown)
        {
            enemy.acttackCoolDown = Random.Range(enemy.minAttackCooldown, enemy.maxAttackCooldown);
            enemy.lastTimeAttack = Time.time;
            return true;
        }
        

        return false;
    }
}
