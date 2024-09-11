using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Crystal_Skill_Controller : MonoBehaviour
{
    private Animator anim => GetComponent<Animator>();
    private Player player;
    private CircleCollider2D cd => GetComponent<CircleCollider2D>();
    private float crystalExistTimer;
    
    private bool canExplode;
    private bool canMove;
    private float moveSpeed;

    private bool canGrow;
    private float growSpeed = 5;
    private Transform closestTarget;
    [SerializeField] private LayerMask whatIsEnemy;
    public void SetUpCrystal(float _crystalDuration,bool _canExplode,bool _canMove,float _moveSpeed,Transform _closestEnemy,Player _player)
    {
        player = _player;
        crystalExistTimer = _crystalDuration;
        canExplode = _canExplode;
        canMove = _canMove;
        moveSpeed = _moveSpeed;
        closestTarget = _closestEnemy;
    }

    public void ChooseRandomEnemy()
    {
        float radius = SkillManager.instance.blackHole.GetBlackHoleRadius();
        Collider2D[] collider = Physics2D.OverlapCircleAll(transform.position, radius,whatIsEnemy);
        if(collider.Length > 0)
        {
            closestTarget = collider[Random.Range(0, collider.Length)].transform;
        }
    }
    public void Update()
    {
        crystalExistTimer -= Time.deltaTime;

        if (crystalExistTimer < 0)
        {
            FinishCrystal();
        }

        if (canMove)
        {
            if (closestTarget == null)
                return;
            transform.position = Vector2.MoveTowards(transform.position,closestTarget.position,moveSpeed * Time.deltaTime);
            if(Vector2.Distance(transform.position,closestTarget.position) < 1)
            {
                canMove = false;
                FinishCrystal();
            }
        }

        if (canGrow) //Thay doi từ scale hiện tại => scale bự hơn (3,3) theo thời gian thực
        {
            transform.localScale = Vector2.Lerp(transform.localScale,new Vector2(3,3),growSpeed * Time.deltaTime);
        }
    }


    private void AnimationExplodeEvent()
    {
        Collider2D[] colliders = Physics2D.OverlapCircleAll(transform.position, cd.radius);
        foreach (var hit in colliders)
        {
            if (hit.GetComponent<Enemy>() != null)
            {
                hit.GetComponent<Entity>().SetupKnockBackDir(transform);
                player.stats.DoMagicDamage(hit.GetComponent<CharacterStats>());

                ItemData_Equipment amuletData = Inventory.instance.GetEquipment(EquipmentType.Amulet);
                if(amuletData != null)            
                    amuletData.Effect(hit.transform);
                
            }
        }
    }

    public void FinishCrystal()
    {
        if (canExplode)
        {
            canGrow = true;
            anim.SetTrigger("Explode");
        }
        else
            SelfDestroy();
    }

    public void SelfDestroy()
    {
        Destroy(gameObject);
    }
}
