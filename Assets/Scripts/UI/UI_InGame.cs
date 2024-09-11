using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UI_InGame : MonoBehaviour
{
    [SerializeField] private PlayerStats playerStats;
    [SerializeField] private Slider slider;

    [SerializeField] private Image dashImage;
    [SerializeField] private Image parryImage;
    [SerializeField] private Image crystalImage;
    [SerializeField] private Image swordImage;
    [SerializeField] private Image blackholeImage;
    [SerializeField] private Image potionImage;

    [SerializeField] private GameObject SkillPage_1;
    [SerializeField] private GameObject SkillPage_2;

    [SerializeField] public GameObject interactButton;

    [Header("Souls info")]
    [SerializeField] private TextMeshProUGUI currentSouls;
    [SerializeField] private float soulsAmount;
    [SerializeField] private float increaseRate = 100;
    private SkillManager skills;
    private PlayerInputManager input;
    void Start()
    {
        if(playerStats != null)
        {
            playerStats.onHealthChanged += UpdateHealthUI;
        }

        if(SkillPage_1.activeSelf && SkillPage_2.activeSelf)
        {
            SkillPage_2.SetActive(false);
        }

        skills = SkillManager.instance;
        input = PlayerInputManager.instance;
    }

  
    void Update()
    {
        UpdateSoulsUI();

        if (input.isDash && skills.dash.dashUnlocked)
            SetCooldownOf(dashImage);

        if (input.isParry && skills.parry.parryUnlocked)
            SetCooldownOf(parryImage);

        if (input.isCrystal && skills.crystal.crystalUnlocked)
            SetCooldownOf(crystalImage);

        if (input.isAim && skills.sword.swordUnlocked)
            SetCooldownOf(swordImage);

        if (input.isBlackHole && skills.blackHole.blackHoleUnlocked)
            SetCooldownOf(blackholeImage);

        if (input.isUseFlask && Inventory.instance.GetEquipment(EquipmentType.Flask) != null)
            SetCooldownOf(potionImage);

        CheckCoolDownOf(dashImage, skills.dash.cooldown);
        CheckCoolDownOf(parryImage, skills.parry.cooldown);
        CheckCoolDownOf(crystalImage, skills.crystal.cooldown);
        CheckCoolDownOf(swordImage, skills.sword.cooldown);
        CheckCoolDownOf(blackholeImage, skills.blackHole.cooldown);
        CheckCoolDownOf(potionImage, Inventory.instance.flaskCooldown);
    }

    #region Press Button
    public void OnJumpPress()
    {
        PlayerInputManager.instance.OnJump(true);
    }

    public void OnJumpRelease()
    {
        PlayerInputManager.instance.OnJump(false);
    }

    public void OnAimPress()
    {
        PlayerInputManager.instance.OnAim(true);
    }

    public void OnAimRelease()
    {
        PlayerInputManager.instance.OnAim(false);
    }

    public void OnDashPress()
    {
        PlayerInputManager.instance.OnDash(true);
    }

    public void OnDashRelease()
    {
        PlayerInputManager.instance.OnDash(false);
    }

    public void OnAttackPress()
    {
        PlayerInputManager.instance.OnAttack(true);
    }

    public void OnAttackRelease()
    {
        PlayerInputManager.instance.OnAttack(false);
    }

    //

    public void OnParryPress()
    {
        PlayerInputManager.instance.OnParry(true);
    }

    public void OnParryRelease()
    {
        PlayerInputManager.instance.OnParry(false);
    }

    public void OnBlackHolePress()
    {
        PlayerInputManager.instance.OnBlackHole(true);
    }

    public void OnBlackHoleRelease()
    {
        PlayerInputManager.instance.OnBlackHole(false);
    }
    public void OnCrystalPress()
    {
        PlayerInputManager.instance.OnCrystal(true);
    }

    public void OnCrystalRelease()
    {
        PlayerInputManager.instance.OnCrystal(false);
    }

    public void OnFlaskPress()
    {
        PlayerInputManager.instance.OnUseFlask(true);
    }
    public void OnFlaskRelease()
    {
        PlayerInputManager.instance.OnUseFlask(false);
    }


    #endregion

    public void SwitchSkillPage()
    {

        if (SkillPage_1.activeSelf && !SkillPage_2.activeSelf)
        {
            SkillPage_1.SetActive(false);
            SkillPage_2.SetActive(true);
        }
        else if (!SkillPage_1.activeSelf && SkillPage_2.activeSelf)
        {
            SkillPage_2.SetActive(false);
            SkillPage_1.SetActive(true);
            
        }
        
    }
    

    private void UpdateSoulsUI()
    {
        float CurrentCurrency = PlayerManager.instance.GetCurrentCurrency();
        if (soulsAmount < CurrentCurrency)
        {
            soulsAmount += Time.deltaTime * increaseRate;
        }
        
        else
            soulsAmount = CurrentCurrency;

        currentSouls.text = ((int)soulsAmount).ToString();
    }

    private void UpdateHealthUI()
    {
        slider.maxValue = playerStats.GetMaxHealthValue();
        slider.value = playerStats.currentHealth;
    }

    private void SetCooldownOf(Image _image)
    {
        if (_image.fillAmount <= 0)
            _image.fillAmount = 1;
    }

    private void CheckCoolDownOf(Image _image,float _cooldown)
    {
        if (_image.fillAmount > 0)
            _image.fillAmount -= 1 / _cooldown * Time.deltaTime;
    }
}
