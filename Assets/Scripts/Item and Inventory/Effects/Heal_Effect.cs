using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Heal Effect", menuName = "Data/Item effect/Heal")]
public class Heal_Effect : ItemEffect
{
    [Range(0f, 1f)]
    [SerializeField] private float healPercent;
    public override void ExecuteEffect(Transform _enemyPosition)
    {
        //player stats
        PlayerStats playerStats = PlayerManager.instance.player.GetComponent<PlayerStats>();

        //how much to heal
        int healAmount = Mathf.RoundToInt( playerStats.GetMaxHealthValue() * healPercent);


        //heal
        playerStats.IncreaseHealthBy(healAmount);
    }
}
