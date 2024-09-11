using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

[CreateAssetMenu(fileName = "Freeze enemy Effect", menuName = "Data/Item effect/Freeze enemy")]

public class FreezeEnemy_Effect : ItemEffect
{
    [SerializeField] private float duration;
    public override void ExecuteEffect(Transform _transform)
    {
        //Set duoi 10% mau thi moi thuc hien effect
        PlayerStats playerStats = PlayerManager.instance.player.GetComponent<PlayerStats>();
        if (playerStats.currentHealth > playerStats.GetMaxHealthValue() * .1f)
            return;

        //Set cooldown cho effect
        if (Inventory.instance.CanUseArmor() == false)  
            return;

        // thuc hien freeze
        Collider2D[] colliders = Physics2D.OverlapCircleAll(_transform.position, 2); 
        foreach (var hit in colliders)
        {
            hit.GetComponent<Enemy>()?.FreezeTimeFor(duration);
           
        }
        
        
    }

}
