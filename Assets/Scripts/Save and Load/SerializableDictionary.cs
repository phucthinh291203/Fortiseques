using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Đối với những dữ liệu phức tạp như Dictionary
//unity không thể trực tiếp chuyển thành dữ liệu json được
//Vì vậy phải serialize hóa ( tuần tự hóa) thông qua các phương thức unity hỗ trợ thì mới lưu được
[System.Serializable]
public class SerializableDictionary<TKey, TValue> : Dictionary<TKey, TValue> , ISerializationCallbackReceiver
{
    [SerializeField] private List<TKey> keys = new List<TKey>();
    [SerializeField] private List<TValue> values = new List<TValue>();

    public void OnBeforeSerialize()     //Quá trình save
    {
        keys.Clear();   //Làm sạch trước khi lưu, vì nó có thể lưu dữ liệu của lần chơi trước
        values.Clear();

        foreach (KeyValuePair<TKey,TValue> pair in this)
        {
            keys.Add(pair.Key);
            values.Add(pair.Value);
        }
    }
    public void OnAfterDeserialize()    //Quá trình load
    {
        this.Clear();

        if (keys.Count != values.Count)
        {
            Debug.Log("Keys count is not equal to values count");
        }

        for (int i = 0; i < keys.Count; i++)
        {
            this.Add(keys[i], values[i]);
        }
    }

}
