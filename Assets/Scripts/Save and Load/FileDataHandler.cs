using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.IO;

public class FileDataHandler 
{
    private string dataDirPath = "";
    private string dataFileName = "";

    private bool encrypted;
    private string codeWord = "phucthinh";

    public FileDataHandler(string _dataDirPath, string _dataFileName,bool _encrypted)
    {
        dataDirPath = _dataDirPath;
        dataFileName = _dataFileName;
        this.encrypted = _encrypted; 
    }

    public void Save(GameData _data)
    {
        string fullPath = Path.Combine(dataDirPath, dataFileName);

        try
        {
            //Xác định đường dẫn
            Directory.CreateDirectory(Path.GetDirectoryName(fullPath));

            //Xác định lưu trữ data dạng json
            string dataToStore = JsonUtility.ToJson(_data, true);

            //Xác định xem dữ liệu có được yêu cầu mã hóa
            
            if (encrypted)
                dataToStore = EncryptDecrypt(dataToStore);

            //Create file tại đường dẫn chỉ định
            using (FileStream stream = new FileStream(fullPath, FileMode.Create))
            {
                using(StreamWriter writer = new StreamWriter(stream))
                {   
                    //Ghi dữ liệu dạng json vào file 
                    writer.Write(dataToStore);
                }
            }
        }

        catch(Exception e)
        {
            Debug.LogError("Error on trying to save data to file: " + fullPath + "\n" + e);
        }
    }

    public GameData Load()
    {
        string fullPath = Path.Combine(dataDirPath, dataFileName);
        Debug.Log(fullPath);
        GameData loadData = null;

        if (File.Exists(fullPath))
        {
            
            try
            {
                string dataToLoad = "";

                using (FileStream stream = new FileStream(fullPath, FileMode.Open))
                {
                    
                    using (StreamReader reader = new StreamReader(stream))
                    {
                        dataToLoad = reader.ReadToEnd();
                        
                    }
                }
               
                if (encrypted)
                    dataToLoad = EncryptDecrypt(dataToLoad);
                

                
                loadData = JsonUtility.FromJson<GameData>(dataToLoad);
                
            }
            catch (Exception e)
            {
                Debug.LogError("Error on trying to load data from file:" + fullPath + "\n" + e);
            }
        }
 
        return loadData;

    }

    public void Delete()
    {
        string fullPath = Path.Combine(dataDirPath, dataFileName);
        try
        {
            if (File.Exists(fullPath))
            {
                File.Delete(fullPath);
                Debug.Log("File deleted successfully: " + fullPath);
            }
            else
            {
                Debug.Log("File not found: " + fullPath);
            }
        }
        catch (Exception e) 
        {
            Debug.LogError("Error deleting file: " + e.Message);
        }
    }

    private string EncryptDecrypt(string _data)
    {
        string modifiedData = "";

        for(int i= 0;i< _data.Length;i++)
        {
            modifiedData += (char)(_data[i] ^ codeWord[i % codeWord.Length]);
        }

        return modifiedData;
    }

    
}
