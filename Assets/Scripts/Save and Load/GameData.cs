using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
//là một thuộc tính (attribute) được sử dụng
//để đánh dấu một lớp hoặc cấu trúc (class or struct) để có thể được tuần tự hóa (serialized).
//Tuần tự hóa là quá trình chuyển đổi một đối tượng thành một định dạng có thể lưu trữ hoặc truyền đi,
//chẳng hạn như lưu vào tệp tin hoặc gửi qua mạng.
public class GameData
{
    public int currency;

    public SerializableDictionary<string, bool> skillTree;
    
    public SerializableDictionary<string, int> inventory;
    public List<string> equipmentId;

    public SerializableDictionary<string, bool> checkpoints;
    public string closestCheckpointId;

    public float lostCurrencyX;
    public float lostCurrencyY;
    public int lostCurrencyAmount;

    public SerializableDictionary<string, float> volumeSetting;
    public GameData()
    {
        this.lostCurrencyX = 0;
        this.lostCurrencyY = 0;
        this.lostCurrencyAmount = 0;

        this.currency = 0;
        
        skillTree = new SerializableDictionary<string, bool>();
        inventory = new SerializableDictionary<string, int>();
        equipmentId = new List<string>();

        checkpoints = new SerializableDictionary<string, bool>();
        closestCheckpointId = string.Empty;

        volumeSetting = new SerializableDictionary<string, float>();
    }
}
