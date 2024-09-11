using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BlackHole_Skill : Skill
{
    [Header("Blackhole info")]
    [SerializeField] private GameObject blackHolePrefab;
    [SerializeField] private float maxSize;
    [SerializeField] private float growSpeed;
    [SerializeField] private float shrinkSpeed;
    [Space]
    [SerializeField] private float blackHoleDuration;
    [SerializeField] private int amountOfAttack = 4;
    [SerializeField] private float cloneAttackCooldown = .3f;
    BlackHole_Skill_Controller currentBlackHole;

    [Header("Blackhole skills")]
    [SerializeField] private UI_SkillTreeSlot blackholeUnlockButton;
    public bool blackHoleUnlocked { get; private set; }
    public override bool CanUseSkill()
    {
        return base.CanUseSkill();
    }
        
    public override void UseSkill()
    {
        base.UseSkill();
        

        GameObject newBlackHole = Instantiate(blackHolePrefab,player.transform.position,Quaternion.identity);
        currentBlackHole = newBlackHole.GetComponent<BlackHole_Skill_Controller>();
        currentBlackHole.SetUpBlackHole(maxSize, growSpeed, shrinkSpeed, amountOfAttack, cloneAttackCooldown,blackHoleDuration);
        AudioManager.instance.PlaySFX(2, null);
        AudioManager.instance.PlaySFX(5, null);
    }

    protected override void Start()
    {
        base.Start();
        blackholeUnlockButton.GetComponent<Button>().onClick.AddListener(UnlockBlackHole);
    }

    protected override void Update()
    {
        base.Update();
    }

    private void UnlockBlackHole()
    {
        if (blackholeUnlockButton.unlocked)
            blackHoleUnlocked = true;
    }

    public bool SkillCompleted()
    {
        if (!currentBlackHole)
            return false;

        if(currentBlackHole.playerCanExitState)
        {
            currentBlackHole = null;
            return true;
        }
        return false;
    }

    public float GetBlackHoleRadius()
    {
        return maxSize / 2;
    }

    protected override void CheckUnlock()
    {
        UnlockBlackHole();
    }
}
