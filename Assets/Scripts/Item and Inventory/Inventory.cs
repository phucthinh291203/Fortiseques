using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class Inventory : MonoBehaviour, ISaveManager
{
    public static Inventory instance;

    public List<ItemData> startingItems;

    public List<InventoryItem> equipment;   //Đang mặc equipment
    public Dictionary<ItemData_Equipment, InventoryItem> equipmentDictionary; //Chứa nhiều món nhưng mỗi món là 1 type khác nhau => 1 type 1 món

    public List<InventoryItem> inventory;   //Đang có trong túi
    public Dictionary<ItemData, InventoryItem> inventoryDictianory; 

    public List<InventoryItem> stash;       //material đang có trong túi
    public Dictionary<ItemData, InventoryItem> stashDictianory;



    [Header("Inventory UI")]

    [SerializeField] private Transform inventorySlotParent;
    [SerializeField] private Transform stashSlotParent;
    [SerializeField] private Transform equpmentSlotParent;
    [SerializeField] private Transform statSlotParent;

    private UI_ItemSlot[] inventoryItemSlot;
    private UI_ItemSlot[] stashItemSlot;
    private UI_EquipmentSlot[] equipmentSlot;
    private UI_StatSlot[] statSlot;

    [Header("Shop UI")]
    UI_ShopList shopUI;

    [Header("Items cooldown")]
    private float lastTimeUsedFlask;
    private float lastTimeUsedArmor;

    public float flaskCooldown { get; private set; }
    private float armorCooldown;

    [Header("Data base")]
    public List<ItemData> itemDataBase;
    public List<InventoryItem> loadedItems; //List này lấy dữ liệu trong database ra -> truyền vô starting item
    public List<ItemData_Equipment> loadedEquipment;
    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
            Destroy(gameObject);
    }

    private void Start()
    {
        inventory = new List<InventoryItem>();
        inventoryDictianory = new Dictionary<ItemData, InventoryItem>();

        stash = new List<InventoryItem>();
        stashDictianory = new Dictionary<ItemData, InventoryItem>();

        equipment = new List<InventoryItem>();
        equipmentDictionary = new Dictionary<ItemData_Equipment, InventoryItem>();


        inventoryItemSlot = inventorySlotParent.GetComponentsInChildren<UI_ItemSlot>();
        stashItemSlot = stashSlotParent.GetComponentsInChildren<UI_ItemSlot>();
        equipmentSlot = equpmentSlotParent.GetComponentsInChildren<UI_EquipmentSlot>();
        statSlot = statSlotParent.GetComponentsInChildren<UI_StatSlot>();

        
        AddStartingItems();
    }

    private void AddStartingItems()
    {
        if(loadedEquipment.Count > 0)
        {
            foreach(ItemData_Equipment equipment in loadedEquipment)
            {
                EquipItem(equipment);
            }
        }


        //Add item từ database vào loadedItems -> loadItems vào startingItem -> startingItem vào túi (stash/equipment)
        if (loadedItems.Count > 0)
        {
         
            foreach (InventoryItem item in loadedItems)
            {
                for (int i = 0; i < item.stackSize; i++) //add số lượng vào cho từng món
                {
                    AddItem(item.data);
                }
            }

            return;
        }


        //Khúc này được sử dụng trong trường hợp new game (ko có load item )
        //Có thể tặng vũ khí cơ bản cho người chơi (tân thủ)
        for (int i = 0; i < startingItems.Count; i++)
        {
            if (startingItems[i] != null)
                AddItem(startingItems[i]);
        }
    }

    public void EquipItem(ItemData _item)
    {
        ItemData_Equipment newEquipment = _item as ItemData_Equipment;  //nếu item là stash sẽ không ép được -> giống việc lọc ra equipment
        InventoryItem newItem = new InventoryItem(newEquipment); //Khởi tạo số lượng

        ItemData_Equipment oldEquipment = null;

        foreach (KeyValuePair<ItemData_Equipment, InventoryItem> item in equipmentDictionary)   //Đổi vũ khí có trong túi khi đang mặc một món khác
        {
            if (item.Key.equipmentType == newEquipment.equipmentType)
                oldEquipment = item.Key;
        }

        if (oldEquipment != null)
        {
            UnequipItem(oldEquipment);  //unequip món đang mặc + loại bỏ món đó khỏi dictionary
            AddItem(oldEquipment);  //add món cũ vào túi đồ lại
        }


        equipment.Add(newItem); //thêm vào list quản lý món đang mặc
        equipmentDictionary.Add(newEquipment, newItem);//thêm vào từ điển quản lý món đang mặc
        newEquipment.AddModifier(); //điều chỉnh chỉ số

        RemoveItem(_item);  //gỡ bỏ món mới khỏi túi đồ, vì nó chuyển qua phần đang mặc

        UpdateSlotUI();
    }

    public void UnequipItem(ItemData_Equipment itemToRemove)
    {
        if (equipmentDictionary.TryGetValue(itemToRemove, out InventoryItem value))
        {
            equipment.Remove(value);
            equipmentDictionary.Remove(itemToRemove);
            itemToRemove.RemoveModifier();
        }
    }

    private void UpdateSlotUI()
    {
        for (int i = 0; i < equipmentSlot.Length; i++)
        {
            foreach (KeyValuePair<ItemData_Equipment, InventoryItem> item in equipmentDictionary)
            {
                if (item.Key.equipmentType == equipmentSlot[i].slotType)
                    equipmentSlot[i].UpdateSlot(item.Value);
            }
        }

        for (int i = 0; i < inventoryItemSlot.Length; i++)
        {
            inventoryItemSlot[i].CleanUpSlot();
        }

        for (int i = 0; i < stashItemSlot.Length; i++)
        {
            stashItemSlot[i].CleanUpSlot();
        }


        for (int i = 0; i < inventory.Count; i++)
        {
            inventoryItemSlot[i].UpdateSlot(inventory[i]);
        }

        for (int i = 0; i < stash.Count; i++)
        {
            stashItemSlot[i].UpdateSlot(stash[i]);
        }

        UpdateStatsUI();
    }

    public void UpdateStatsUI()
    {
        for (int i = 0; i < statSlot.Length; i++) // update info of stats in character UI
        {
            statSlot[i].UpdateStatValueUI();
        }
    }

    public void AddItem(ItemData _item) //Phân loại món đồ rồi add vào chỗ tương ứng
    {
        if (_item.itemType == ItemType.Equipment && CanAddItem())
            AddToInventory(_item);
        else if (_item.itemType == ItemType.Material)
            AddToStash(_item);



        UpdateSlotUI();
    }

    private void AddToStash(ItemData _item)
    {
        if (stashDictianory.TryGetValue(_item, out InventoryItem value))
        {
            value.AddStack();
        }
        else
        {
            InventoryItem newItem = new InventoryItem(_item);
            stash.Add(newItem);
            stashDictianory.Add(_item, newItem);
        }
    }

    private void AddToInventory(ItemData _item)
    {
        if (inventoryDictianory.TryGetValue(_item, out InventoryItem value))
        {
            value.AddStack();
        }
        else
        {
            InventoryItem newItem = new InventoryItem(_item);
            inventory.Add(newItem);
            inventoryDictianory.Add(_item, newItem);
        }
    }

    public void RemoveItem(ItemData _item)
    {
        if (inventoryDictianory.TryGetValue(_item, out InventoryItem value))
        {
            if (value.stackSize <= 1)
            {
                inventory.Remove(value);
                inventoryDictianory.Remove(_item);
            }
            else
                value.RemoveStack();
        }


        if (stashDictianory.TryGetValue(_item, out InventoryItem stashValue))
        {
            if (stashValue.stackSize <= 1)
            {
                stash.Remove(stashValue);
                stashDictianory.Remove(_item);
            }
            else
                stashValue.RemoveStack();
        }

        UpdateSlotUI();
    }

    public bool CanAddItem()
    {
        if (inventory.Count >= inventoryItemSlot.Length)
        {
            return false;       //Da full tui
        }

        return true;        //Chua full tui
    }

    //public bool CanCraft(ItemData_Equipment _itemToCraft, List<InventoryItem> _requiredMaterials)
    //{
    //    List<InventoryItem> materialsToRemove = new List<InventoryItem>();

    //    for (int i = 0; i < _requiredMaterials.Count; i++)
    //    {
    //        if (stashDictianory.TryGetValue(_requiredMaterials[i].data, out InventoryItem stashValue))
    //        {
    //            if (stashValue.stackSize < _requiredMaterials[i].stackSize)
    //            {
    //                Debug.Log("Not enough materials");
    //                return false;
    //            }
    //            else
    //            {
    //                materialsToRemove.Add(stashValue);
    //            }

    //        }
    //        else
    //        {
    //            Debug.Log("Materials not found");
    //            return false;
    //        }
    //    }

    //    for (int i = 0; i < materialsToRemove.Count; i++)
    //    {
    //        RemoveItem(materialsToRemove[i].data);
    //    }


    //    AddItem(_itemToCraft);
    //    Debug.Log("Here is your item " + _itemToCraft.name);

    //    return true;
    //}

    public bool CanCraft(ItemData_Equipment _itemToCraft, List<InventoryItem> _requiredMaterials)
    {
        List<InventoryItem> materialsToRemove = new List<InventoryItem>();

        for (int i = 0; i < _requiredMaterials.Count; i++)  //Check xem trong túi có đủ nl yêu cầu ko
        {
            InventoryItem requiredItem = _requiredMaterials[i];
            if (stashDictianory.TryGetValue(requiredItem.data, out InventoryItem stashValue)) //Nếu có thì lấy số lượng kiểm tra
            {
                if (stashValue.stackSize < requiredItem.stackSize) // ko đủ sl so với yêu cầu
                {
                    Debug.Log("Not enough materials for: " + requiredItem.data.name);
                    return false;
                }
                else
                {
                    // Khởi tạo InventoryItem với tham số đúng
                    // Tạo một InventoryItem mới với số lượng cần thiết
                    InventoryItem itemToRemove = new InventoryItem(requiredItem.data) //đủ thì tạo 1 biến ảo chứa thông tin material sẽ xóa
                    {
                        stackSize = requiredItem.stackSize
                    };
                    materialsToRemove.Add(itemToRemove);
                }
            }
            else
            {
                Debug.Log("Material not found: " + requiredItem.data.name);
                return false;
            }
        }

        // Liệt kê các nguyên liệu cần loại bỏ
        foreach (var item in materialsToRemove)
        {
            Debug.Log("Material to remove: " + item.data.name + ", Quantity: " + item.stackSize);
        }

        // Loại bỏ nguyên liệu trong kho
        foreach (var item in materialsToRemove)
        {
            //// Đảm bảo giảm số lượng đúng với số lượng cần loại bỏ
            if (stashDictianory.TryGetValue(item.data, out InventoryItem stashItem))
            {

                stashItem.stackSize -= item.stackSize;
                if (stashItem.stackSize <= 0)
                {
                    stash.Remove(stashItem);
                    stashDictianory.Remove(item.data);
                }
            }
        }

        // Thêm vật phẩm đã chế tạo
        AddItem(_itemToCraft);
        Debug.Log("Here is your item: " + _itemToCraft.name);

        return true;
    }

    public bool CanBuy(ItemData _item)
    {
        if (_item != null)
        {
            bool isEquipment = _item is ItemData_Equipment;
            if(PlayerManager.instance.currency >= _item.price) //Đủ tiền
            {
                if(!isEquipment || CanAddItem())    //Nguyên liệu hoặc vũ khí / vũ khí sẽ bị full inventory
                {
                    Debug.Log("Da mua thanh cong item " + _item);
                    PlayerManager.instance.DecreaseCurrency(_item.price);
                    AddItem(_item);
                    return true;
                }
            }
        }
            return false;
    }


    public List<InventoryItem> GetEquipmentList() => equipment;

    public List<InventoryItem> GetStashList() => stash;

    public ItemData_Equipment GetEquipment(EquipmentType _type)
    {
        ItemData_Equipment equipedItem = null;

        foreach (KeyValuePair<ItemData_Equipment, InventoryItem> item in equipmentDictionary)
        {
            if (item.Key.equipmentType == _type)
                equipedItem = item.Key;
        }

        return equipedItem;
    }

    public void UseFlask()
    {
        ItemData_Equipment currentFlask = GetEquipment(EquipmentType.Flask);

        if (currentFlask == null)
            return;

        bool canUseFlask = Time.time > lastTimeUsedFlask + flaskCooldown;

        if (canUseFlask)
        {
            flaskCooldown = currentFlask.itemCooldown;
            currentFlask.Effect(null);
            lastTimeUsedFlask = Time.time;
        }
        else
            Debug.Log("Flask on cooldown;");
    }

    public bool CanUseArmor()
    {
        ItemData_Equipment currentArmor = GetEquipment(EquipmentType.Armor);

        if (Time.time > lastTimeUsedArmor + armorCooldown)
        {
            armorCooldown = currentArmor.itemCooldown;
            lastTimeUsedArmor = Time.time;
            return true;
        }

        Debug.Log("Armor on cooldown");
        return false;
    }

    public void LoadData(GameData _data)
    {
        Debug.Log("Data của inventory có load");
        foreach (KeyValuePair<string, int> pair in _data.inventory)
        {
            
            foreach (var item in itemDataBase)
            {
                if (item != null && item.itemId == pair.Key)    //Vì trong database nó lưu item và FOLDER luôn, nên cần lọc FOLDER ra
                {
                    InventoryItem itemToLoad = new InventoryItem(item);
                    itemToLoad.stackSize = pair.Value;

                    loadedItems.Add(itemToLoad);
                }
            }
        }

        foreach(string loadedItemId in _data.equipmentId)
        {
            Debug.Log("Có data");
            foreach(var item in itemDataBase)
            {
                if(item != null && loadedItemId == item.itemId)
                {
                    Debug.Log("Có vào if");
                    loadedEquipment.Add(item as ItemData_Equipment);
                }
            }
        }
       
    }

    public void SaveData(ref GameData _data)
    {
        _data.inventory.Clear();
        _data.equipmentId.Clear();

        foreach (KeyValuePair<ItemData, InventoryItem> pair in inventoryDictianory)
        {
            _data.inventory.Add(pair.Key.itemId, pair.Value.stackSize);
        }

        foreach(KeyValuePair<ItemData, InventoryItem> pair in stashDictianory)
        {
            _data.inventory.Add(pair.Key.itemId, pair.Value.stackSize);
        }

        foreach(KeyValuePair<ItemData_Equipment,InventoryItem> pair in equipmentDictionary)
        {
            _data.equipmentId.Add(pair.Key.itemId);
        }
    }
#if UNITY_EDITOR

    [ContextMenu("Fill up item data base")]
    private void FillUpItemDataBase() => itemDataBase = new List<ItemData>(GetItemDataBase());
    //Lấy toàn bộ thông tin vật phẩm có trong folder game lưu vào
    private List<ItemData> GetItemDataBase()
    {
        List<ItemData> itemDataBase = new List<ItemData>();
        string[] assetNames = AssetDatabase.FindAssets("", new[] { "Assets/Scripts/Data/Items" });

        foreach (string SOName in assetNames)
        {
            var SOpath = AssetDatabase.GUIDToAssetPath(SOName);
            var itemData = AssetDatabase.LoadAssetAtPath<ItemData>(SOpath);
            itemDataBase.Add(itemData);
        }

        return itemDataBase;
    }
#endif
}
