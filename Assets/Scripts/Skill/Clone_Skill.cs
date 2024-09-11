using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class Clone_Skill : Skill
{
    [Header("Clone info")]
    [SerializeField] float attackMutiplier; //Tấn công của clone scale theo tấn công của người chơi
    [SerializeField] GameObject clonePrefab;
    [SerializeField] private float cloneDuration;
    [Space]

    [Header("Clone attack")]  //Clone có thể attack enemy (stats tấn công của clone = 80% của player)    
    //(có thể gây effect dựa trên sát thương nguyên tố) (tùy thuộc vào dam nguyên tố của player)
    [SerializeField] private UI_SkillTreeSlot cloneAttackUnlockButton;
    [SerializeField] private float cloneAttackMultiplier;
    [SerializeField] bool canBeAttack;

    [Header("Agressive clone")]  //Clone có thể tạo effect từ món vũ khí player đang mặc
    [SerializeField] private UI_SkillTreeSlot agressiveCloneUnlockButton;
    [SerializeField] private float agressiveAttackMultilier;
    public bool canApplyOnHitEffect { get; private set; }


    [Header("Multiple Clone")]
    [SerializeField] private UI_SkillTreeSlot multipleUnlockButton;
    [SerializeField] bool canDuplicateClone;
    [SerializeField] float chanceToDuplicateClone;
    [SerializeField] private float multiCloneAttackMultiplier;

    [Header("Crystal instead of Clone")]
    [SerializeField] private UI_SkillTreeSlot crystalInsteadUnlockButton;
    public bool crystalInsteadOfClone;

    protected override void Start()
    {
        base.Start();
        cloneAttackUnlockButton.GetComponent<Button>().onClick.AddListener(UnlockCloneAttack);
        agressiveCloneUnlockButton.GetComponent<Button>().onClick.AddListener(UnlockAggressiveClone);
        multipleUnlockButton.GetComponent<Button>().onClick.AddListener(UnlockMultipleClone);
        crystalInsteadUnlockButton.GetComponent<Button>().onClick.AddListener(UnlockCrystalInstead);
    }


    #region Unlock region

    protected override void CheckUnlock()
    {
        UnlockCloneAttack();
        UnlockAggressiveClone();
        UnlockMultipleClone();
        UnlockCrystalInstead();
    }
    private void UnlockCloneAttack()
    {
        if (cloneAttackUnlockButton.unlocked)
        {
            canBeAttack = true;
            attackMutiplier = cloneAttackMultiplier;
        }
    }


    private void UnlockAggressiveClone()
    {
        if (agressiveCloneUnlockButton.unlocked)
        {
            canApplyOnHitEffect = true;
            attackMutiplier = agressiveAttackMultilier;
        }
    }

    private void UnlockMultipleClone()
    {
        if (multipleUnlockButton.unlocked)
        {
            canDuplicateClone = true;
            attackMutiplier = multiCloneAttackMultiplier;
        }
    }

    private void UnlockCrystalInstead()
    {
        if (crystalInsteadUnlockButton.unlocked)
        {
            crystalInsteadOfClone = true;
        }
    }
    #endregion


    public void CreateClone(Transform _clonePostion, Vector3 _offset)
    {
        if (crystalInsteadOfClone)
        {
            SkillManager.instance.crystal.CreateCrystal();
            return;
        }
        GameObject newClone = Instantiate(clonePrefab);
        newClone.GetComponent<Clone_Skill_Controller>().SetUpClone(_clonePostion, cloneDuration, canBeAttack, _offset, FindClosestEnemy(newClone.transform), canDuplicateClone, chanceToDuplicateClone, player, attackMutiplier);
    }



    public void CreateCloneWithDelay(Transform _enemy)
    {
        StartCoroutine(CloneDelayCoroutine(_enemy, new Vector3(2 * player.facingDirection, 0, 0)));
    }

    public IEnumerator CloneDelayCoroutine(Transform _transform, Vector3 _offset)
    {
        yield return new WaitForSeconds(.4f);
        player.skill.clone.CreateClone(_transform, _offset);

    }
}
