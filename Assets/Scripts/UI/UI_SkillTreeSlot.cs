using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class UI_SkillTreeSlot : MonoBehaviour,IPointerEnterHandler,IPointerExitHandler,ISaveManager
{

    private UI ui;
    [SerializeField] private int skillPrice;

    [SerializeField] private string skillName;
    [TextArea]
    [SerializeField] private string skillDescription;
    [SerializeField] private Color lockSkillColor;

    public bool unlocked;
    [SerializeField] private UI_SkillTreeSlot[] shouldBeUnlocked; //Phải được mở khóa trước đó
    [SerializeField] private UI_SkillTreeSlot[] shouldBeLocked;

    private Image skillImage;

    private void OnValidate()
    {
        gameObject.name = "SkillTreeSlotUI - " + skillName;
    }

    private void Awake()
    {
        GetComponent<Button>().onClick.AddListener(() => UnlockAkillSlot());
    }

    private void Start()
    {
        skillImage = GetComponent<Image>();
        skillImage.color = lockSkillColor;
        ui = GetComponentInParent<UI> ();

        
    }

    private void Update()
    {
        if (unlocked == true)
        {
            skillImage.color = Color.white;
        }
    }
    public void UnlockAkillSlot()
    {
        if (PlayerManager.instance.HaveEnoughMoney(skillPrice) == false)
            return;

            for (int i = 0; i < shouldBeUnlocked.Length; i++)
            {
                if (shouldBeUnlocked[i].unlocked == false)
                {
                    Debug.Log("Cannot unlock skill");
                    return;
                }
            }

            for (int i = 0; i < shouldBeLocked.Length; i++)
            {
                if (shouldBeLocked[i].unlocked == true)
                {
                    Debug.Log("Cannot unlock skill");
                    return;
                }
            }

            unlocked = true;
            skillImage.color = Color.white;
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        ui.skillToolTip.ShowToolTip(skillDescription,skillName,skillPrice);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        ui.skillToolTip.HideToolTip();
    }

    public void LoadData(GameData _data)
    {
        if (_data.skillTree.TryGetValue(skillName, out bool value))
        {
            unlocked = value;
        }
    }

    public void SaveData(ref GameData _data)
    { 
        if (_data.skillTree.TryGetValue(skillName, out bool value))
        {
            _data.skillTree.Remove(skillName);
            _data.skillTree.Add(skillName, unlocked);

        }
        else
        {
            _data.skillTree.Add(skillName, unlocked);
        }
    }
}
