using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Clone_Skill_Controller : MonoBehaviour
{
    private Player player;
    private SpriteRenderer sr;
    [SerializeField] private float colorLoosingSpeed;
    [SerializeField] private Transform attackCheck;
    private float attackCheckRadius = 0.8f;
    private Animator anim;
    private float cloneTimer;
    private float attackMultiplier;
    private Transform closetEnemy;
    private bool canDuplicateClone;
    private int faceDirection = 1;
    private float chanceToDuplicate;
    private void Awake()
    {
        sr = GetComponent<SpriteRenderer>();
        anim = GetComponent<Animator>();
    }

    private void Update()
    {
        cloneTimer -= Time.deltaTime;
        if(cloneTimer < 0)
        {
            //Hinh anh clone se mo dan
            sr.color = new Color(1, 1, 1, sr.color.a - (Time.deltaTime * colorLoosingSpeed));

            if (sr.color.a < 0)
                Destroy(gameObject);
        }
    }
    public void SetUpClone(Transform _newTransform,float _cloneDuration,bool _canBeAttack,Vector3 _offset,Transform _closestEnemy,bool _canDuplicate,float _chanceToDuplicate,Player _player,float _attackMultiplier)
    {
        if (_canBeAttack)
            anim.SetInteger("AttackNumber", Random.Range(1, 3));

        player = _player;
        transform.position  = _newTransform.position + _offset;
        
        closetEnemy = _closestEnemy;
        cloneTimer = _cloneDuration;
        canDuplicateClone = _canDuplicate;
        chanceToDuplicate = _chanceToDuplicate;
        attackMultiplier = _attackMultiplier;
        FaceClosestTarget();
        
    }

    private void AnimationTrigger() 
    {
        cloneTimer = -1f;
    }

    private void AttackTrigger() // Thuc hien danh duoc quai
    {
        Collider2D[] colliders = Physics2D.OverlapCircleAll(attackCheck.position, attackCheckRadius);
        foreach (var hit in colliders)
        {
            if (hit.GetComponent<Enemy>() != null)
            {
                //player.stats.DoDamage(hit.GetComponent<CharacterStats>()); //Thay thế bằng hàm khác, vì clone sẽ lấy stats tấn công của player để làm dam của nó
                hit.GetComponent<Entity>().SetupKnockBackDir(transform);
                PlayerStats playerStats = player.GetComponent<PlayerStats>();
                EnemyStats enemyStats = hit.GetComponent<EnemyStats>();

                playerStats.CloneDoDamage(enemyStats, attackMultiplier);

                if (player.skill.clone.canApplyOnHitEffect)
                {
                    ItemData_Equipment weaponData = Inventory.instance.GetEquipment(EquipmentType.Weapon);

                    if (weaponData != null)
                        weaponData.Effect(hit.transform);
                }

                if (canDuplicateClone)
                {
                    if (Random.Range(0, 100) < chanceToDuplicate)
                    {
                        SkillManager.instance.clone.CreateClone(hit.transform, new Vector3(.5f * faceDirection,0));
                    }
                }
            }
        }
    }

    private void FaceClosestTarget()
    {
        if(closetEnemy != null) // Kiem tra thay doi tuong gan nhat (Phat hien enemy trong ban kinh)
        {
            if (transform.position.x > closetEnemy.position.x)
            {
                faceDirection = -1;
                transform.Rotate(0, 180, 0); // xoay lai
                // neu ma vi tri clone no nam truoc vt enemy

            } 
        }
    }
}
