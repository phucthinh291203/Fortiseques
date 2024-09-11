using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.UI;

public class Crystal_Skill : Skill
{
    [SerializeField] private float crystalDuration;
    [SerializeField] private GameObject crystalPrefab;
    private GameObject currentCrystal;

    [Header("Crystal Mirage")]
    [SerializeField] private UI_SkillTreeSlot unlockCloneInsteadButton;
    [SerializeField] private bool cloneInsteadOfCrystal;

    [Header("Crystal Simple")]
    [SerializeField] private UI_SkillTreeSlot unlockCrystalButton;
    public bool crystalUnlocked { get; private set; }

    [Header("Explosive crystal")]
    [SerializeField] private UI_SkillTreeSlot unlockExplosiveButton;
    [SerializeField] private bool canExplode;

    [Header("Moving crystal")]
    [SerializeField] private UI_SkillTreeSlot unlockMovingCrystalButton;
    [SerializeField] private bool canMoveToEnemy;
    [SerializeField] float moveSpeed;

    [Header("Multiple crystal")]
    [SerializeField] private UI_SkillTreeSlot unlockMultipleCrystalButton;
    [SerializeField] private bool canUseMultiStack;
    [SerializeField] private int amountOfStacks;
    [SerializeField] private float multiStackCooldown;
    [SerializeField] private float useTimeWindow;
    [SerializeField] private List<GameObject> crystalLeft = new List<GameObject>();

    protected override void Start()
    {
        base.Start();
        unlockCrystalButton.GetComponent<Button>().onClick.AddListener(UnlockCrystal);
        unlockCloneInsteadButton.GetComponent<Button>().onClick.AddListener(UnlockCrystalMirage);
        unlockExplosiveButton.GetComponent<Button>().onClick.AddListener(UnlockExplosiveCrystal);
        unlockMovingCrystalButton.GetComponent<Button>().onClick.AddListener(UnlockMovingCrystal);
        unlockMultipleCrystalButton.GetComponent<Button>().onClick.AddListener(UnlockMultipleCrystal);
        Debug.Log("Nạp skill từ UI_SkillTree");
    }

    //Mở khóa skill crystal
    #region Unlock Skill

    protected override void CheckUnlock()
    {
        UnlockCrystal();
        UnlockCrystalMirage();
        UnlockExplosiveCrystal();
        UnlockMovingCrystal();
        UnlockMultipleCrystal();
    }
    private void UnlockCrystal()
    {

        if (unlockCrystalButton.unlocked)
            crystalUnlocked = true;
        else Debug.Log("Nút chưa được mở khóa");
    }

    private void UnlockCrystalMirage()
    {
        if (unlockCloneInsteadButton.unlocked)
            cloneInsteadOfCrystal = true;
    }

    private void UnlockExplosiveCrystal()
    {
        if (unlockExplosiveButton.unlocked)
            canExplode = true;
    }

    private void UnlockMovingCrystal()
    {
        if (unlockMovingCrystalButton.unlocked)
            canMoveToEnemy = true;
    }

    private void UnlockMultipleCrystal()
    {
        if (unlockMultipleCrystalButton.unlocked)
            canUseMultiStack = true;
    }

    #endregion 
    public override void UseSkill()
    {
        base.UseSkill();
        Debug.Log("Use skill crystal");
        Debug.Log(currentCrystal == null ? "null":"Duoc bam f");
        if (CanUseMultiCrystal())
        {
            Debug.Log("multi stack");
            return;
        }

        if(currentCrystal == null) //Nhan F de dung skill
        {
            CreateCrystal();
        }
        else //Nhan F them 1 lần nữa (trong khi co crystal de teleport)  
        {
            if (canMoveToEnemy)
                return;
            //Hoan doi vi tri giua player va crystal (giong hoan vi A và B)
            Vector2 playerPos = player.transform.position;
            player.transform.position = currentCrystal.transform.position;
            currentCrystal.transform.position = playerPos;


            if (cloneInsteadOfCrystal) // nếu chọn nhánh skill tree clone attack khi hoán đổi vt
            {
                SkillManager.instance.clone.CreateClone(currentCrystal.transform, Vector3.zero);
                Destroy(currentCrystal);
            }
            else // neu chọn nhánh skill tree crystal phát nổ khi hoán đổi vt
            {

                //Hoan doi vi tri xong thi crystal phat no
                currentCrystal.GetComponent<Crystal_Skill_Controller>()?.FinishCrystal();
            }
        }
    }

    public void CreateCrystal()
    {
        currentCrystal = Instantiate(crystalPrefab, player.transform.position, Quaternion.identity);
        Crystal_Skill_Controller currentCrystalScript = currentCrystal.GetComponent<Crystal_Skill_Controller>();
        currentCrystalScript.SetUpCrystal(crystalDuration, canExplode, canMoveToEnemy, moveSpeed, FindClosestEnemy(currentCrystal.transform),player);

        
        
    }

    public void CurrentCrystalChooseRandomEnemy()
    {
        currentCrystal.GetComponent<Crystal_Skill_Controller>().ChooseRandomEnemy();
    }


    private bool CanUseMultiCrystal()
    {
        if (canUseMultiStack)
        {
            if(crystalLeft.Count > 0) 
            {
                Debug.Log("Có vào đây");
                if(crystalLeft.Count == amountOfStacks)
                {
                    Invoke("ResetAbility", useTimeWindow);
                }
                cooldown = 0;
                GameObject crystalToSpawn = crystalLeft[crystalLeft.Count - 1];
                GameObject newCrystal = Instantiate(crystalToSpawn,player.transform.position,Quaternion.identity);

                crystalLeft.Remove(crystalToSpawn);

                newCrystal.GetComponent<Crystal_Skill_Controller>().
                    SetUpCrystal(crystalDuration, canExplode, canMoveToEnemy, moveSpeed, FindClosestEnemy(newCrystal.transform), player); 
            
            

                if(crystalLeft.Count <= 0)
                {
                    //cooldown the skill
                    //refill crystals

                    cooldown = multiStackCooldown;
                    RefillCrystal();
                }

            return true;    

            }
        }


        return false;
    }

    private void RefillCrystal()
    {
        int amountToAdd = amountOfStacks - crystalLeft.Count;
        for (int i = 0; i < amountToAdd; i++)
        {
            crystalLeft.Add(crystalPrefab);
        }
    }

    private void ResetAbility()
    {
        if (cooldownTimer > 0)
            return;
        cooldownTimer = multiStackCooldown;
        RefillCrystal();
    }
}
