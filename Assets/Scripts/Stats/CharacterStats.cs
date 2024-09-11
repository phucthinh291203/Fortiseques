
using System.Collections;
using System.Net;
using Unity.VisualScripting.Antlr3.Runtime.Misc;
using UnityEngine;


public enum statType
{
    strength,
    agility,
    intelligence,
    vitality,
    damage,
    critChance,
    critPower,
    maxHealth,
    armor,
    evasion,
    magicRes,
    fireDamage,
    iceDamage,
    lightingDamage
}

public class CharacterStats : MonoBehaviour
{
    private EntityFx fx;

    [Header("Major stats")]
    public Stat strength; //1 point increase damage by 1 and crit_power by 1%
    public Stat agility; //1 point increase evasion by 1% and crit_chance by 1%
    public Stat intelligence;//1 point increase magic damage by 1 and magic resistance by 3
    public Stat vitality;// 1 point increase health by 3 or 5 points

    [Header("Offensive stats")]
    public Stat damage;
    public Stat critChance;
    public Stat critPower;          // default value 150%


    [Header("Defensive stats")]
    public Stat maxHealth;
    public Stat evasion;
    public Stat armor;
    public Stat magicResistance;

    [Header("Magic stats")]
    public Stat fireDamage;         //does damage over time
    public Stat iceDamage;          //reduce armor by 20%
    public Stat lightingDamage;     //reduce accuracy by 20%

    public bool isIgnited;
    public bool isChill;
    public bool isShocked;

    
    [SerializeField] private float ailmentsDuration = 4f;
    private float ignitedTimer;      //thoi gian bi thieu dot (vd: bi dot trong 4 giay)
    private float ignitedDamageCooldown = .3f;   //bien chua thoi gian  truyen vao -> khoang cach
    private float ignitedDamageTimer;    //Khoang cach giua moi lan dot 
    private int ignitedDamage;       //Luong damage gay ra moi giay (vd 1 giay gây 5 damage)

    [SerializeField] private GameObject shockStrikePrefab;
    public bool isDead { get; private set; }
    public bool isInvicible { get; private set; }
    private bool isVulnerable;
    private int shockDamage;


    private float chillTimer;
    private float shockTimer;

    public int currentHealth;
    


    public System.Action onHealthChanged;
    // Start is called before the first frame update
    protected virtual void Start()
    {
        currentHealth = GetMaxHealthValue();
        critPower.SetDefaultValue(150);
        fx = GetComponent<EntityFx>();
    }

    protected virtual void Update()
    {
        ignitedTimer -= Time.deltaTime;
        ignitedDamageTimer -= Time.deltaTime;

        chillTimer -= Time.deltaTime;
        shockTimer -= Time.deltaTime;

        if (ignitedTimer < 0)
            isIgnited = false;

        if (chillTimer < 0)
            isChill = false;

        if (shockTimer < 0)
            isShocked = false;

        if(isIgnited)
            ApplyIgniteDamage();

    }

    private IEnumerator VulnerableCoroutine(float _duration)
    {
        isVulnerable = true;
        yield return new WaitForSeconds(_duration);

        isVulnerable = false;

    }

    public void MakeVulnerableFor(float _duration)
    {
        StartCoroutine(VulnerableCoroutine(_duration));
    }

    public virtual void DoDamage(CharacterStats _targetStats)
    {
        bool criticalStrike = false;
        if (TargetCanAvoidAttack(_targetStats))
            return;

        _targetStats.GetComponent<Entity>().SetupKnockBackDir(transform);
        
        int totalDamage = damage.GetValue() + strength.GetValue();

        if (CanCrit())
        {
            totalDamage += CalculateCriticalDamage(totalDamage);
            criticalStrike = true;
        }
         
        fx.CreateHitFx(_targetStats.transform,criticalStrike);

        totalDamage = CheckTargetArmor(_targetStats, totalDamage);

        _targetStats.TakeDamage(totalDamage);

        //if inventory current weapon has fire effect
        DoMagicDamage(_targetStats);
    }

    #region Magical damage and ailments

    public virtual void DoMagicDamage(CharacterStats _targetStats)
    {
        int _fireDamage = fireDamage.GetValue();
        int _iceDamage = iceDamage.GetValue();
        int _lightingDamage = lightingDamage.GetValue();


        int totalMagicDamage = _fireDamage + _iceDamage + _lightingDamage + intelligence.GetValue();
        totalMagicDamage = CheckTargetResistance(_targetStats, totalMagicDamage);

        _targetStats.TakeDamage(totalMagicDamage);

        if (Mathf.Max(_fireDamage, _iceDamage, _lightingDamage) <= 0)
            return;
        AttempApplyAilment(_targetStats, _fireDamage, _iceDamage, _lightingDamage);

    }

    private void AttempApplyAilment(CharacterStats _targetStats, int _fireDamage, int _iceDamage, int _lightingDamage)
    {
        bool canApplyIgnite = _fireDamage > _iceDamage && _fireDamage > _lightingDamage;
        bool canApplyChill = _iceDamage > _fireDamage && _iceDamage > _lightingDamage;
        bool canApplyShock = _lightingDamage > _fireDamage && _lightingDamage > _iceDamage;

        while (!canApplyIgnite && !canApplyChill && !canApplyShock)
        {
            if (Random.value < 0.33f && _fireDamage > 0)
            {
                canApplyIgnite = true;
                _targetStats.ApplyAilment(canApplyIgnite, canApplyChill, canApplyShock);
                Debug.Log("Apply fire");
                return;
            }

            if (Random.value < 0.33f && _iceDamage > 0)
            {
                canApplyChill = true;
                _targetStats.ApplyAilment(canApplyIgnite, canApplyChill, canApplyShock);
                Debug.Log("Apply ice");
                return;
            }

            if (Random.value < 0.33f && _lightingDamage > 0)
            {
                canApplyShock = true;
                _targetStats.ApplyAilment(canApplyIgnite, canApplyChill, canApplyShock);
                Debug.Log("Apply light");
                return;
            }
        }

        if (canApplyIgnite)
        {
            _targetStats.SetUpIgniteDamage(Mathf.RoundToInt(_fireDamage * .2f));
        }

        if (canApplyShock)
        {
            _targetStats.SetUpStrikeDamage(Mathf.RoundToInt(_lightingDamage * .1f));
        }

        _targetStats.ApplyAilment(canApplyIgnite, canApplyChill, canApplyShock);
    }

    public void ApplyAilment(bool _isIgnited, bool _isChill, bool _isShocked)
    {
        bool canApplyIgnite = !isIgnited && !isChill&& !_isShocked; //ca 3 hieu ung phai dang false
        bool canApplyChill = !isIgnited && !isChill && !_isShocked; //ca 3 hieu ung phai dang false
        bool canApplyShock = !isIgnited && !isChill; //lua va bang phai false, shock co the true hoac false
        //Shock effect la ban ra sam set nen co the thuc hien tiep tuc neu no true

        if (_isIgnited && canApplyIgnite)
        {
            isIgnited = _isIgnited;
            ignitedTimer = ailmentsDuration;
            fx.IgniteFxFor(ailmentsDuration);
        }

        if (_isChill && canApplyChill)
        {
            isChill = _isChill;
            chillTimer = ailmentsDuration;
            float slowPercentage = .2f;
            GetComponent<Entity>().SlowEntityBy(slowPercentage, ailmentsDuration); 
            fx.ChillFxFor(ailmentsDuration);
        }

        if (_isShocked && canApplyShock)
        {
            if (!isShocked)                         // quai bi effect lan dau -> giam accuracy
            {
                ApplyShock(_isShocked);
            }
            else                                   // quai bi danh tiep khi effect van con -> ban ra sam set
            {
                //find closest target, only 1 enemy
                //instantiate thunder strike
                //set up thunder

                if (GetComponent<Player>() != null)  //truong hop quai danh player, thi set danh enemy
                    return;
                HitNearestTargetWithShockStrike();
            }

        }

    }

    public void ApplyShock(bool _isShocked)
    {
        if (isShocked) // tranh truong hop quai thu 2 trung set, thi no ban set them quai thu 3
            return;

        isShocked = _isShocked;
        shockTimer = ailmentsDuration;
        fx.ShockFxFor(ailmentsDuration);
    }

    private void ApplyIgniteDamage()
    {
        if (ignitedDamageTimer < 0)
        {
            Debug.Log("Take burn damage: " + ignitedDamage);

            DecreaseHealthBy(ignitedDamage);
            if (currentHealth < 0 && !isDead)
                Die();

            ignitedDamageTimer = ignitedDamageCooldown;
        }
    }

    private void HitNearestTargetWithShockStrike()
    {
        Collider2D[] colliders = Physics2D.OverlapCircleAll(transform.position, 25); //Kiem tra co enemy nao trong khu vuc khong
        float closetDistance = Mathf.Infinity;
        Transform closetEnemy = null;
        foreach (var hit in colliders) //Giong thuat toan tim GiaTriMin trong 1 mang 
        {
            if (hit.GetComponent<Enemy>() != null && Vector2.Distance(transform.position, hit.transform.position) > 1)
            {
                float distanceToEnemy = Vector2.Distance(transform.position, hit.transform.position);

                if (distanceToEnemy < closetDistance)// Mục đích là check xem closetDistance có chứa vi tri enemy chưa
                {
                    closetDistance = distanceToEnemy; // luu khoang cach tu clone den enemy
                    closetEnemy = hit.transform; //luu vi tri enemy dang dung
                }
            }

            if (closetEnemy == null)   //optinal :neu khong co quai nao ben canh thi danh enemy gan nhat
                closetEnemy = transform;
        }

        if (closetEnemy != null)
        {
            GameObject newShockStrike = Instantiate(shockStrikePrefab, transform.position, Quaternion.identity);
            newShockStrike.GetComponent<ShockStrike_Controller>().SetUp(shockDamage, closetEnemy.GetComponent<CharacterStats>());
        }
    }

    public void SetUpIgniteDamage(int _damage)
    {
        ignitedDamage = _damage;
    }

    public void SetUpStrikeDamage(int _damage)
    {
        shockDamage = _damage;
    }

    #endregion

    public virtual void TakeDamage(int _damage)
    {
        if (isInvicible)
            return;

        DecreaseHealthBy(_damage);

        GetComponent<Entity>().DamageImpact();
        fx.StartCoroutine("FlashFx");

        if (currentHealth <= 0 && !isDead) {
            Die();
        }
    }

    public virtual void IncreaseStatsBy(int _modifier,float _duration,Stat _statToModify)
    {
        //Chay buff hieu ung trong khoang thoi gian quy dinh
        StartCoroutine(StatModifyCouroutine(_modifier,_duration,_statToModify));
    }

    private IEnumerator StatModifyCouroutine(int _modifier, float _duration, Stat _statToModify)
    {
        _statToModify.AddModifier(_modifier);
        yield return new WaitForSeconds(_duration);
        _statToModify.RemoveModifier(_modifier);
    }

    public virtual void IncreaseHealthBy(int _amount)
    {
        currentHealth += _amount;
        if(currentHealth > GetMaxHealthValue())     //Hoi full thanh mau thi dung
        {
            currentHealth = GetMaxHealthValue();    //Ko duoc vuot qua max health
        }

        if (onHealthChanged != null)
        {
            onHealthChanged();
        }
    }

    protected virtual void DecreaseHealthBy(int _damage)
    {
        if (isVulnerable)
            _damage =  Mathf.RoundToInt(_damage * 1.1f);

        currentHealth -= _damage;
        if (onHealthChanged != null)
            onHealthChanged();
    }

    protected virtual void Die()
    {
        isDead = true;
    }

    #region Stat calculations

    public virtual void OnEvasion()
    {
        
    }

    protected bool TargetCanAvoidAttack(CharacterStats _targetStats)
    {
        int totalEvasion = _targetStats.evasion.GetValue() + _targetStats.agility.GetValue();

        if (isShocked)
            totalEvasion += 20;
        if (Random.Range(0, 100) < totalEvasion)
        {
            _targetStats.OnEvasion();
            return true;
        }
        return false;
    }
    protected int CheckTargetArmor(CharacterStats _targetStats, int totalDamage)
    {
        if (isChill)
            totalDamage -= Mathf.RoundToInt(_targetStats.armor.GetValue() * .8f);
        else
            totalDamage -= _targetStats.armor.GetValue();

        totalDamage = Mathf.Clamp(totalDamage, 0, int.MaxValue);
        Debug.Log("total damage with armor: " + totalDamage);
        return totalDamage;
    }
    private int CheckTargetResistance(CharacterStats _targetStats, int totalMagicDamage)
    {
        totalMagicDamage -= _targetStats.magicResistance.GetValue() + (_targetStats.intelligence.GetValue() * 3);
        totalMagicDamage = Mathf.Clamp(totalMagicDamage, 0, int.MaxValue);
        return totalMagicDamage;
    }

    protected bool CanCrit()
    {
        int totalCriticalChance = critChance.GetValue() + agility.GetValue();
        if( Random.Range(0,100) <= totalCriticalChance)
        { 
            return true;
        }
        return false;
    }

    protected int CalculateCriticalDamage(int _damage)
    {
        float totalCritPower = (critPower.GetValue() + strength.GetValue()) * .01f;
        Debug.Log("total crit power % " + totalCritPower);

        float critDamage = _damage * totalCritPower;
        Debug.Log("total crit damamge before round up " + critDamage);

        return Mathf.RoundToInt(critDamage);
    } 

    public int GetMaxHealthValue()
    {
        return maxHealth.GetValue() + (vitality.GetValue() * 5);
    }
    #endregion

    public Stat GetType(statType statType)
    {
        if (statType == statType.strength) return strength;
        else if (statType == statType.agility) return agility;
        else if (statType == statType.intelligence) return intelligence;
        else if (statType == statType.vitality) return vitality;
        else if (statType == statType.damage) return damage;
        else if (statType == statType.critChance) return critChance;
        else if (statType == statType.critPower) return critPower;
        else if (statType == statType.maxHealth) return maxHealth;
        else if (statType == statType.armor) return armor;
        else if (statType == statType.evasion) return evasion;
        else if (statType == statType.magicRes) return magicResistance;
        else if (statType == statType.fireDamage) return fireDamage;
        else if (statType == statType.iceDamage) return iceDamage;
        else if (statType == statType.lightingDamage) return lightingDamage;

        return null;
    }


    public void KillEntity()
    {
        if(!isDead)
            Die();
    }

    public void SetInvicible(bool _invicible)
    {
        isInvicible = _invicible;
    }
}
