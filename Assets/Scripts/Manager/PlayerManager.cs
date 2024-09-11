using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices.WindowsRuntime;
using UnityEngine;

public class PlayerManager : MonoBehaviour,ISaveManager
{
    public static PlayerManager instance;
    public Player player;

    public int currency;
    private void Awake()
    {
        if(instance != null)
        {
            Destroy(instance.gameObject);
        }
        else
        instance = this;
    }

    public bool HaveEnoughMoney(int _price)
    {
        if (_price > currency)
        {
            Debug.Log("Not enough currency");
            return false;
        }
        else
        {
            currency -= _price;
            return true;
        }
    }

    public int GetCurrentCurrency()
    {
        return currency;
    }

    public void DecreaseCurrency(int _price)
    {
        currency -= _price;
    }

    public void IncreaseCurrency(int _price)
    {
        currency += _price;
    }

    public void LoadData(GameData _data)
    {
        this.currency = _data.currency;
    }

    public void SaveData(ref GameData _data)    //Tham chiếu đến GameData
    {
        _data.currency = this.currency;
    }
}
