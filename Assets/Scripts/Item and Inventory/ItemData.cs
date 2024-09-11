using System.Text;
using UnityEditor;
using UnityEngine;

public enum ItemType
{
    Material,
    Equipment
}


[CreateAssetMenu(fileName = "New Item Data", menuName = "Data/Item")]
public class ItemData : ScriptableObject
{
    public ItemType itemType;
    public string itemName;
    public Sprite icon;
    public string itemId;

    public int price;
    [TextArea]
    public string describeItem;
    [Range(0,100)]
    public int dropChance;

    protected StringBuilder sb = new StringBuilder();

    private void OnValidate()
    {
#if UNITY_EDITOR                //Cái này chỉ chạy trong Unity_editor
        string path = AssetDatabase.GetAssetPath(this); 
        itemId = AssetDatabase.AssetPathToGUID(path);
#endif
    }
    public virtual string GetDescription()
    {
        if (sb != null)
        {
            sb = new StringBuilder();
        }

        if (describeItem == null)
            return string.Empty;
        sb.Length = 0;

        sb.Append(describeItem);
        sb.AppendLine();
        Debug.Log(sb.ToString());
        return sb.ToString();
    }
}
