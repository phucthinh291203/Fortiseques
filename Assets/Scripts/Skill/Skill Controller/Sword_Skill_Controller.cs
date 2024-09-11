using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Sword_Skill_Controller : MonoBehaviour
{
    private Animator anim;
    private Rigidbody2D rb;
    private CircleCollider2D cd;
    private Player player;
    private bool canRotate = true;
    private bool isReturning;

    private float freezeTimeDuration;
    private float returningSpeed = 12f;

    [Header("Spin info")]
    private float maxTravelDistance;
    private float spinDuration;
    private float spinTimer;
    private bool wasStopped;
    private bool isSpining;

    private float hitTimer;
    private float hitCoolDown;

    private float spinDirection;

    [Header("Pierce info")]
    private float pierceAmount;
    private float pierceGravity;


    [Header("Bounce info")]
    private float bouncingSpeed;
    private bool isBouncing;
    private int bounceAmount;
    private List<Transform> enemyTarget;
    private int targetIndex;

    private void Awake()
    {
        anim = GetComponentInChildren<Animator>();
        rb = GetComponent<Rigidbody2D>();
        cd= GetComponent<CircleCollider2D>();
    }

    public void SetUpBounce(bool _isBouncing,int _amountOfBounce,float _bouncingSpeed)
    {
        isBouncing = _isBouncing;
        bounceAmount = _amountOfBounce;
        bouncingSpeed = _bouncingSpeed;
        enemyTarget = new List<Transform>();
    }

    public void SetUpPierce(int _pierceAmount)
    {
        pierceAmount = _pierceAmount;
    }

    public void SetUpSpin(bool _isSpining,float _maxTravelDistance,float _spinDuration,float _hitCoolDown)
    {
        isSpining = _isSpining;
        maxTravelDistance = _maxTravelDistance;
        spinDuration = _spinDuration; 
        hitCoolDown = _hitCoolDown;
    }

    public void DestroyMe()
    {
        Destroy(gameObject);
    }

    public void SetUpSword(Vector2 dir,float _gravity,Player _player,float _freezeTimeDuration,float _returningSpeed)
    {
        player = _player;
        freezeTimeDuration = _freezeTimeDuration;
        returningSpeed = _returningSpeed;
        rb.velocity = new Vector2(player.facingDirection * 50,0); //dir;
        rb.gravityScale = 0; //_gravity;
        
        if(pierceAmount <=0)
            anim.SetBool("Rotation", true);

        spinDirection = Mathf.Clamp(rb.velocity.x, -1, 1);

        Invoke("DestroyMe", 7);
    }

    public void Update()
    {
        if (canRotate)
            transform.right = rb.velocity;
        if (isReturning)
        {
            transform.position = Vector2.MoveTowards(transform.position, player.transform.position, returningSpeed * Time.deltaTime);
            if (Vector2.Distance(transform.position, player.transform.position) < 0.5)
                player.CatchTheSword();
        }

        BounceLogic();
        SpinLogic();
    }

    private void SpinLogic()
    {
        if (isSpining)
        {
            if (Vector2.Distance(player.transform.position, transform.position) > maxTravelDistance && !wasStopped)
            {
                StopWhenSpining();
            }

            if (wasStopped)     //Khi cay kiem ngung tai vi tri cach player 
            {
                //transform.position = Vector2.MoveTowards(transform.position, new Vector2(transform.position.x + spinDirection, transform.position.y), 1.5f * Time.deltaTime);

                spinTimer -= Time.deltaTime;   //Ket thuc thoi gian thi kiem ngung xoay, quay ve player
                if (spinTimer < 0)
                {
                    isReturning = true;
                    isSpining = false;
                }

                hitTimer -= Time.deltaTime;   //khoảng cách thoi gian cay kiếm gây dam mỗi lần xoay
                if (hitTimer < 0)
                {
                    hitTimer = hitCoolDown;
                    Collider2D[] colliders = Physics2D.OverlapCircleAll(transform.position, 1f); //Tim kiem quai
                    foreach (var hit in colliders)
                    {
                        if (hit.GetComponent<Enemy>() != null)
                            SwordSkillDamage(hit.GetComponent<Enemy>());// gay dam cho enemy
                            
                    }
                }


            }
        }
    }

    private void StopWhenSpining()
    {
        wasStopped = true;
        rb.constraints = RigidbodyConstraints2D.FreezePosition;
        spinTimer = spinDuration;
    }

    private void BounceLogic()
    {
        if (isBouncing && enemyTarget.Count > 0)
        {
            transform.position = Vector2.MoveTowards(transform.position, enemyTarget[targetIndex].position, bouncingSpeed * Time.deltaTime);
            if (Vector2.Distance(transform.position, enemyTarget[targetIndex].position) < .1f) //Kiềm tra khoang cach giữa con quái đầu và quái tiếp theo
            {
                SwordSkillDamage(enemyTarget[targetIndex].GetComponent<Enemy>());        
                targetIndex++;
                if (targetIndex >= enemyTarget.Count)
                    targetIndex = 0;

                bounceAmount--;
                if (bounceAmount == 0)
                {
                    isBouncing = false;
                    isReturning = true;
                }


            }
        }
    }

    public void ReturnSword()
    {
        rb.constraints = RigidbodyConstraints2D.FreezeAll;
        rb.isKinematic = false;
        transform.parent = null;
        isReturning = true;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (isReturning) // nếu return sword khi nó chưa chạm vào enemy hay mặt đất
        {              // (chủ yếu để giữ hình kiếm vẫn xoay)
            return;
        }

        if(collision.GetComponent<Enemy>() != null)
        {
            Enemy enemy = collision.GetComponent<Enemy>();
            SwordSkillDamage(enemy);

        }
        SetUpTargetForBounce(collision);

        StuckInto(collision);
    }

    private void SwordSkillDamage(Enemy enemy)
    {

        EnemyStats enemyStats = enemy.GetComponent<EnemyStats>();
        player.stats.DoDamage(enemyStats);

        if(player.skill.sword.timeStopUnlock) //Quai bi đóng băng
            enemy.FreezeTimeFor(freezeTimeDuration);

        if (player.skill.sword.vulnerableUnlock) 
        {
            enemyStats.MakeVulnerableFor(freezeTimeDuration);
        }
            
        ItemData_Equipment amuletData = Inventory.instance.GetEquipment(EquipmentType.Amulet);
        if(amuletData != null)
        {
            amuletData.Effect(enemy.transform);
        }
    }

    private void SetUpTargetForBounce(Collider2D collision)
    {
        if (collision.GetComponent<Enemy>() != null)  // nếu trúng con quái đầu tiên
        {
            if (isBouncing && enemyTarget.Count <= 0) // danh sách target đang rỗng và bouncing = true
            {
                Collider2D[] colliders = Physics2D.OverlapCircleAll(transform.position, 10); // Con quái đầu tiên sẽ dò xung quanh
                foreach (var hit in colliders)
                {
                    if (hit.GetComponent<Enemy>() != null) // dò xem xung quanh nó có còn quái nào nữa không
                        enemyTarget.Add(hit.transform); // thêm dữ liệu toàn bộ con quái xung quanh vào list
                }
            }
        }
    }

    private void StuckInto(Collider2D collision)
    {
        if (isSpining)
        {
            StopWhenSpining();
            return;
        }
         if(pierceAmount > 0 && collision.GetComponent<Enemy>() != null)
        {
            pierceAmount--;
            return;
        }

        canRotate = false;
        cd.enabled = false;
        rb.isKinematic = true;
        rb.constraints = RigidbodyConstraints2D.FreezeAll; //freeze trục x,y,z 
        GetComponentInChildren<ParticleSystem>().Play();
        if (isBouncing && enemyTarget.Count > 0)
            return;

        transform.parent = collision.transform;
        //transform.parent là lấy transform cha của sword ( toàn bộ object sword)
        //collision.transform là lấy transform của collider mà nó vừa đụng vào
        anim.SetBool("Rotation", false);
    }
}
