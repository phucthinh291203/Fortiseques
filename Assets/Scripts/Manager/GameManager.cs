using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour,ISaveManager
{
    public static GameManager instance;
    [Header("Check point")]
    [SerializeField] private CheckPoint[] checkPoints;
    [SerializeField] private string closestCheckpointId;

    [Header("Lost currency")]
    [SerializeField] private GameObject lostCurrencyPrefab;
    public int lostCurrencyAmount;
    [SerializeField] private float lostCurrencyX;
    [SerializeField] private float lostCurrencyY;

    private Transform player;

    public void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
            Destroy(instance.gameObject);
    }

    private void Start()
    {
        player = PlayerManager.instance.player.transform;
        checkPoints = FindObjectsOfType<CheckPoint>();
    }
    public void RestartScene()
    {
        SaveManager.instance.SaveGame();
        Scene scene = SceneManager.GetActiveScene();
        SceneManager.LoadScene(scene.name);
    }

    public void SaveAndQuit()
    {
        SaveManager.instance.SaveGame();
        Time.timeScale = 1;
        SceneManager.LoadScene("MainMenu");
    }

    public void LoadData(GameData _data)
    {
        StartCoroutine(LoadWithDelay(_data)); //Phải delay vì PlayerManager khởi tạo ko kịp so với CheckPoint
    }

    private void LoadAllCheckpoints(GameData _data)
    {
        foreach (KeyValuePair<string, bool> pair in _data.checkpoints)
        {
            foreach (CheckPoint checkPoint in checkPoints)
            {
                if (checkPoint.checkpointId == pair.Key && pair.Value == true)
                    checkPoint.ActivateCheckpoint();
            }
        }
    }

    private void LoadLostCurrency(GameData _data)
    {
        lostCurrencyAmount = _data.lostCurrencyAmount;
        lostCurrencyX = _data.lostCurrencyX;
        lostCurrencyY = _data.lostCurrencyY;
        if(lostCurrencyAmount > 0)
        {
            GameObject newLostCurrency = Instantiate(lostCurrencyPrefab, new Vector2(lostCurrencyX, lostCurrencyY), Quaternion.identity);
            newLostCurrency.GetComponent<LostCurrencyController>().dropCurrency = lostCurrencyAmount;
        }

        lostCurrencyAmount = 0;
    }

    private IEnumerator LoadWithDelay(GameData _data)
    {
        yield return new WaitForSeconds(.1f);
        LoadAllCheckpoints(_data);
        LoadPlayerToClosestCheckpoint(_data);
        LoadLostCurrency(_data);
    }

    public void SaveData(ref GameData _data)
    {

        _data.lostCurrencyAmount = lostCurrencyAmount;
        _data.lostCurrencyX = player.position.x;
        _data.lostCurrencyY = player.position.y;

        if(FindClosestCheckpoint() != null)     //Nếu nhân vật chưa qua checkpoint nào hết thì ko cần lưu id
            _data.closestCheckpointId = FindClosestCheckpoint().checkpointId;

        _data.checkpoints.Clear();
        foreach(CheckPoint _checkPoint in checkPoints)
        {
            _data.checkpoints.Add(_checkPoint.checkpointId, _checkPoint.activationStatus);
        }
    }

    private void LoadPlayerToClosestCheckpoint(GameData _data)
    {
        if (closestCheckpointId == null)
            return;
        
        closestCheckpointId = _data.closestCheckpointId;

        foreach (CheckPoint checkpoint in checkPoints)
        {
            if (closestCheckpointId == checkpoint.checkpointId)
            {
                PlayerManager.instance.player.transform.position = checkpoint.transform.position;//main reason
            }
        }
    }


    public CheckPoint FindClosestCheckpoint()
    {
        float closestDistance = Mathf.Infinity;
        CheckPoint closestcheckPoint = null;
        foreach(CheckPoint checkpoint in checkPoints)
        {
            float distanceToCheckpoint = Vector2.Distance(player.position, checkpoint.transform.position);
            if(distanceToCheckpoint < closestDistance && checkpoint.activationStatus == true)
            {
                closestDistance = distanceToCheckpoint;
                closestcheckPoint = checkpoint;
            }

        }
        return closestcheckPoint;
    }

    public void PauseGame(bool _pause)
    {
        if (_pause)
            Time.timeScale = 0;
        else
            Time.timeScale = 1;
    }
}
