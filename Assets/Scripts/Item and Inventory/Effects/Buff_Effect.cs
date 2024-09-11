using System.Collections;
using System.Collections.Generic;
using UnityEngine;



[CreateAssetMenu(fileName = "Buff Effect", menuName = "Data/Item effect/Buff")]

public class Buff_Effect : ItemEffect
{
    private PlayerStats stats;
    [SerializeField] private statType buffType;
    [SerializeField] private int buffAmount;
    [SerializeField] private float buffDuration;

    public override void ExecuteEffect(Transform _enemyPosition)
    {
        stats = PlayerManager.instance.player.GetComponent<PlayerStats>();
        stats.IncreaseStatsBy(buffAmount, buffDuration, stats.GetType(buffType));
    }

}
