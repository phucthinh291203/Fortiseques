using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class SaveManager : MonoBehaviour
{
    public static SaveManager instance;

    [SerializeField] private string fileName;
    [SerializeField] private bool encrypted;
    private GameData gameData;
    private List<ISaveManager> saveManagers;
    private FileDataHandler dataHandler;
    

    [ContextMenu("Delete Save File")]
    public void DeleteSaveData()
    {
        
        dataHandler = new FileDataHandler(/*"D:\\Unity Project\\file_save"*/  Application.persistentDataPath, fileName, encrypted);
        dataHandler.Delete();
    }

    private void Awake()    //singleton
    {
        if (instance != null)
            Destroy(instance.gameObject);

        else
        { 
            instance = this;
        }


        saveManagers = FindAllSaveManagers();
    }


    private void Start()
    {
        dataHandler = new FileDataHandler(Application.persistentDataPath, fileName, encrypted);
        LoadGame();
    }

    public void NewGame()
    {
        gameData = new GameData();
    }

    public void LoadGame()
    {
        Debug.Log("Thực hiện load sau khi quay về menu");
        gameData = dataHandler.Load();  //Lấy dữ liệu từ json về

        if (this.gameData == null)
        {
            Debug.Log("No saved data found!");
            NewGame();
        }

        foreach(ISaveManager saveManager in saveManagers)
        {
            saveManager.LoadData(gameData); 
        }
    }

    public void SaveGame()
    {
        foreach(ISaveManager saveManager in saveManagers)
        {
            saveManager.SaveData(ref gameData);
        }
        dataHandler.Save(gameData); //Tất cả đều save vào cùng 1 đối tượng
        
    }

    
    private void OnApplicationQuit()
    {
        SaveGame();
    }

    //Tạo ra cái này để yêu cầu tất cả những đã assign interface save&load,
    //Từ đó yêu cầu tất cả thực hiện save&load cùng lúc, thay vì qua từng manager để gọi
    //IEnumerable<ISaveManager>: Được sử dụng khi chỉ cần duyệt qua các phần tử và không cần thay đổi dữ liệu.
    //List : được sử dụng nếu muốn thay đổi dữ liệu trong danh sách ấy
    private List<ISaveManager> FindAllSaveManagers()    
    {
        IEnumerable<ISaveManager> saveManagers = FindObjectsOfType<MonoBehaviour>().OfType<ISaveManager>();
        //return new List<ISaveManager>(saveManagers);
        List<ISaveManager> saveManagerList = new List<ISaveManager>(saveManagers);

        // Tìm đối tượng cụ thể có script UI_SkillTreeSlot và thêm nó vào danh sách
        UI_SkillTreeSlot[] skillTreeSlots = FindObjectsOfType<UI_SkillTreeSlot>();
        foreach (var skillTreeSlot in skillTreeSlots)
        {
            saveManagerList.Add(skillTreeSlot);
        }

       
        return saveManagerList;
    }


    public bool hasSaveData()
    {
        dataHandler = new FileDataHandler(Application.persistentDataPath, fileName, encrypted);
        if (dataHandler.Load() != null)
        {
            return true;
        }

        return false;
    }
}
