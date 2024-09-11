using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Ice and Fire effect", menuName = "Data/Item effect/Ice and Fire")]

public class IceAndFire_Effect : ItemEffect
{
    [SerializeField] private GameObject iceAndFirePrefab;
    [SerializeField] private float xVelocity;
    
    public override void ExecuteEffect(Transform _respawnPosition)
    {
        Player player = PlayerManager.instance.player;
        bool thirdAttack = player.primaryAttackState.comboCounter == 2;


        if (thirdAttack)
        {
            GameObject newIceAndFire = Instantiate(iceAndFirePrefab, _respawnPosition.position, player.transform.rotation);
            Rigidbody2D rb = newIceAndFire.GetComponent<Rigidbody2D>();
            rb.velocity = new Vector2(xVelocity * player.facingDirection,0);
            Destroy(newIceAndFire, 5f);
        }
    }

}
