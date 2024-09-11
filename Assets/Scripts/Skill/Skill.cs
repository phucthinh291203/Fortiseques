using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Skill : MonoBehaviour
{
    public float cooldown;
    public float cooldownTimer;
    protected Player player;
    private bool showCooldownTextFx;
    protected virtual void Start()
    {
        player = PlayerManager.instance.player;

        Invoke("CheckUnlock",0.25f);
    }

    protected virtual void Update()
    {
        cooldownTimer -= Time.deltaTime;
    }

    protected virtual void CheckUnlock()
    {

    }
    public virtual bool CanUseSkill()
    {
        if(cooldownTimer < 0)
        {
            //use skill
            UseSkill();
            cooldownTimer = cooldown;
            showCooldownTextFx = false;
            return true;
            
        }
        if (!showCooldownTextFx)
        {
            player.fx.CreatePopUpText("Cooldown");
            showCooldownTextFx = true;
        }
        return false;
    }

    public virtual void UseSkill()
    {
        //do some skill

    }

    protected virtual Transform FindClosestEnemy(Transform _checkTranform) 
    {
        Collider2D[] colliders = Physics2D.OverlapCircleAll(_checkTranform.position, 25); //Kiem tra co enemy nao trong khu vuc khong
        float closetDistance = Mathf.Infinity;
        Transform closetEnemy = null;
        foreach (var hit in colliders) //Giong thuat toan tim GiaTriMin trong 1 mang 
        {
            if (hit.GetComponent<Enemy>() != null)
            {
                float distanceToEnemy = Vector2.Distance(_checkTranform.position, hit.transform.position);

                if (distanceToEnemy < closetDistance)// Mục đích là check xem closetDistance có chứa vi tri enemy chưa
                {
                    closetDistance = distanceToEnemy; // luu khoang cach tu clone den enemy
                    closetEnemy = hit.transform; //luu vi tri enemy dang dung
                }
            }
        }
        return closetEnemy;
    }
}
