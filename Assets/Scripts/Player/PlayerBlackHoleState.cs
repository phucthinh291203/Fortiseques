using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerBlackHoleState : PlayerState
{
    private float flyTime = .4f;
    private bool skillUsed;
    private float gravityDefault = 1f;
    public PlayerBlackHoleState(Player _player, PlayerStateMachine _stateMachine, string _animBoolName) : base(_player, _stateMachine, _animBoolName)
    {


    }

    public override void Enter()
    {
        base.Enter();
        gravityDefault = rb.gravityScale;
        skillUsed = false;
        stateTimer = flyTime;
        rb.gravityScale = 0;
        
    }

    public override void Exit()
    {
        base.Exit();
        PlayerManager.instance.player.fx. MakeTransparent(false);
        rb.gravityScale = gravityDefault;
    }

    public override void Update()
    {
        base.Update();
        if(stateTimer > 0) 
            rb.velocity = new Vector2(0, 15);  //Nhân vật bay từ từ lên (cho ngầu)
        if(stateTimer < 0)
        {
            rb.velocity = new Vector2(0, -.1f);  //Xuống chầm chậm theo thời gian (cho nó ngầu)

            if (!skillUsed)
            {
                if(player.skill.blackHole.CanUseSkill())
                     skillUsed = true;
            }
        }

        if (player.skill.blackHole.SkillCompleted())
            stateMachine.ChangeState(player.airState);   
    }
}


   

