using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

public class UI_StatSlot : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField] private statType statType;
    [SerializeField] private string statName;
    [SerializeField] private TextMeshProUGUI statValueText;
    [SerializeField] private TextMeshProUGUI statNameText;

    [TextArea]
    [SerializeField] private string statDescription;

    public UI ui;

    private void OnValidate()
    {
        gameObject.name = "Stat: - " + statName;
        if (statValueText != null)
            statNameText.text = statName;
    }
    // Start is called before the first frame update
    void Start()
    {
        ui = GetComponentInParent<UI>();
        UpdateStatValueUI();
    }

    public void UpdateStatValueUI()
    {
        PlayerStats playerStats = PlayerManager.instance.player.GetComponent<PlayerStats>();
        if (playerStats != null)
        {
            statValueText.text = playerStats.GetType(statType).GetValue().ToString();
        }

        if (statType == statType.damage)
            statValueText.text = (playerStats.damage.GetValue() + playerStats.strength.GetValue()).ToString();
        if (statType == statType.maxHealth)
            statValueText.text = (playerStats.GetMaxHealthValue()).ToString();
        if (statType == statType.critPower)
            statValueText.text = (playerStats.strength.GetValue() + playerStats.critPower.GetValue()).ToString();
        if(statType == statType.critChance)
            statValueText.text = (playerStats.agility.GetValue() + playerStats.critChance.GetValue()).ToString();
        if (statType == statType.evasion)
            statValueText.text = (playerStats.agility.GetValue() + playerStats.evasion.GetValue()).ToString();
        if(statType == statType.magicRes)
            statValueText.text = (playerStats.magicResistance.GetValue() + (playerStats.intelligence.GetValue() * 3)).ToString();
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        ui.statToolTip.ShowStatToolTip(statDescription);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        ui.statToolTip.HideStatToolTip();
    }
}
