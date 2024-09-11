using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class UI_SkillToolTip : UI_ToolTip
{
    [SerializeField] private TextMeshProUGUI skillDescription;
    [SerializeField] private TextMeshProUGUI skillName;
    [SerializeField] private TextMeshProUGUI skillCost;
    [SerializeField] private float defaultFontSize;
    public void ShowToolTip(string _skillDescription,string _skillName,int _skillPrice)
    {
        skillName.text = _skillName;
        skillDescription.text = _skillDescription;
        skillCost.text = "Cost: " + _skillPrice;
        AdjustPosition();
        AdjustFontSize(skillName);
        gameObject.SetActive(true);
    }

    public void HideToolTip()
    {
        skillName.fontSize = defaultFontSize;
        gameObject.SetActive(false);
    }
}
