using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyState
{
    public Enemy enemyBase;
    public EnemyStateMachine stateMachine{ get;private set; }
    protected Rigidbody2D rb;
    private string animBoolName;

    protected bool triggerCalled;
    protected float stateTimer;
    public EnemyState(Enemy _enemyBase, EnemyStateMachine _stateMachine, string _anim)
    {
        this.enemyBase = _enemyBase;
        this.stateMachine = _stateMachine;
        this.animBoolName = _anim;
    }

    public virtual void Update()
    {
        stateTimer -= Time.deltaTime;
    }
    public virtual void Enter()
    {
        triggerCalled = false;
        rb = enemyBase.rb;
        enemyBase.anim.SetBool(animBoolName, true);
    }

    public virtual void Exit()
    {
        enemyBase.anim.SetBool(animBoolName, false);
        enemyBase.AssignLastAnimName(animBoolName);
    }

    public virtual void AnimationFinishTrigger()
    {
        triggerCalled = true;
    }


}
