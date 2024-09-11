using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerGroundState : PlayerState
{
    public PlayerGroundState(Player _player, PlayerStateMachine _stateMachine, string _animBoolName) : base(_player, _stateMachine, _animBoolName)
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

        if (Time.timeScale == 0)
            return;

        if (PlayerInputManager.instance.isBlackHole && player.skill.blackHole.blackHoleUnlocked)//Hố đen
        {
            if (player.skill.blackHole.cooldownTimer > 0)
                return;
            stateMachine.ChangeState(player.blackHoleState);
        }   
        
        if (PlayerInputManager.instance.isAim && HasNoSword() && player.skill.sword.swordUnlocked) //Ném kiếm
            stateMachine.ChangeState(player.aimSwordState);

        if(PlayerInputManager.instance.isParry && player.skill.parry.parryUnlocked) //Phản đòn
            stateMachine.ChangeState(player.counterAttackState);

        if (PlayerInputManager.instance.isAttack) {                  
            stateMachine.ChangeState(player.primaryAttackState);//Tấn công thường
        }

        if (!player.IsGroundDetect())                          //Rơi từ trên xuống
            stateMachine.ChangeState(player.airState);

        if ((PlayerInputManager.instance.canJump || Input.GetKeyDown(KeyCode.Space)) && player.IsGroundDetect())
        {
            stateMachine.ChangeState(player.jumpState);
        }

    }


    public bool HasNoSword()
    {
        if (!player.sword)
        {
            return true;
        }
        player.sword.GetComponent<Sword_Skill_Controller>().ReturnSword();
        return false;
    }
}
