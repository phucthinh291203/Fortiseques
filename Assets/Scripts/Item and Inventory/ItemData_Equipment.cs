using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

public enum EquipmentType
{
    Weapon,
    Armor,
    Amulet,
    Flask
}

[CreateAssetMenu(fileName = "New Item Data", menuName = "Data/Equipment")]
public class ItemData_Equipment : ItemData
{
    [Header("Unit effect")]
    public EquipmentType equipmentType;
    public ItemEffect[] itemEffect;
    public float itemCooldown;

    

    private int descriptionLength;
    public List<InventoryItem> craftingMaterials;

    [Header("Major stats")]
    public int strength;
    public int agility;
    public int intelligence;
    public int vitality;

    [Header("Offensive stats")]
    public int damage;
    public int critChance;
    public int critPower;

    [Header("Defensive stats")]
    public int health;
    public int armor;
    public int evasion;
    public int magicResistance;

    [Header("Magic stats")]
    public int fireDamage;
    public int iceDamage;
    public int lightingDamage;

    public void Effect(Transform _enemyPosition)
    {
        foreach(var item in itemEffect)
        {
            item.ExecuteEffect(_enemyPosition);
        }
    }
    public void AddModifier()
    {
        PlayerStats playerStats = PlayerManager.instance.player.GetComponent<PlayerStats>();

        playerStats.strength.AddModifier(strength);
        playerStats.agility.AddModifier(agility);
        playerStats.intelligence.AddModifier(intelligence);
        playerStats.vitality.AddModifier(vitality);

        playerStats.damage.AddModifier(damage);
        playerStats.critChance.AddModifier(critChance);
        playerStats.critPower.AddModifier(critPower);

        playerStats.maxHealth.AddModifier(health);
        playerStats.armor.AddModifier(armor);
        playerStats.evasion.AddModifier(evasion);
        playerStats.magicResistance.AddModifier(magicResistance);

        playerStats.fireDamage.AddModifier(fireDamage);
        playerStats.iceDamage.AddModifier(iceDamage);
        playerStats.lightingDamage.AddModifier(lightingDamage);
    }

    public void RemoveModifier() 
    {
        PlayerStats playerStats = PlayerManager.instance.player.GetComponent<PlayerStats>();

        playerStats.strength.RemoveModifier(strength);
        playerStats.agility.RemoveModifier(agility);
        playerStats.intelligence.RemoveModifier(intelligence);
        playerStats.vitality.RemoveModifier(vitality);

        playerStats.damage.RemoveModifier(damage);
        playerStats.critChance.RemoveModifier(critChance);
        playerStats.critPower.RemoveModifier(critPower);

        playerStats.maxHealth.RemoveModifier(health);
        playerStats.armor.RemoveModifier(armor);
        playerStats.evasion.RemoveModifier(evasion);
        playerStats.magicResistance.RemoveModifier(magicResistance);

        playerStats.fireDamage.RemoveModifier(fireDamage);
        playerStats.iceDamage.RemoveModifier(iceDamage);
        playerStats.lightingDamage.RemoveModifier(lightingDamage);
    }


    public override string GetDescription()
    {
        string baseDescription = base.GetDescription();
        Debug.Log(baseDescription);
        if (sb != null)
        {
            Debug.LogError("StringBuilder sb is null");
            sb = new StringBuilder();
        }

        

        if (itemEffect == null)
        {
            Debug.LogError("itemEffect is null");
            return string.Empty;
        }

        sb.Length = 0;
        descriptionLength = 0;
        sb.Append(baseDescription);
        
        AddItemDescription(strength, "Strength");
        AddItemDescription(agility, "Agility");
        AddItemDescription(intelligence, "Intelligence");
        AddItemDescription(vitality, "Vitality");

        AddItemDescription(damage, "Damage");
        AddItemDescription(critChance, "Crit chance");
        AddItemDescription(critPower, "Crit power");

        AddItemDescription(health, "Health");
        AddItemDescription(armor, "Armor");
        AddItemDescription(evasion, "Evasion");
        AddItemDescription(magicResistance, "Magic Res");

        AddItemDescription(fireDamage, "Fire Damage");
        AddItemDescription(iceDamage, "Ice Damage");
        AddItemDescription(lightingDamage, "Lighting Damage");

        
        for(int i = 0; i < itemEffect.Length; i++)
        { 
            if (itemEffect[i].effectDescription.Length > 0)
            {
                sb.AppendLine();
                sb.AppendLine("Unique: " + itemEffect[i].effectDescription);
                descriptionLength++;
            }
        }

        if(descriptionLength < 5)
        {
            for(int i = 0;i < 5 - descriptionLength;i++)
            {
                sb.AppendLine();
                sb.Append("");
            }
        }

        

        return sb.ToString();
    }

    private void AddItemDescription(int _value,string _name)
    {
        if(_value != 0)
        {
            if(sb.Length > 0)
                sb.AppendLine();

            if (_value > 0)
                sb.Append("+ "  + _value + " " + _name);

            descriptionLength++;
        }
    }
}
