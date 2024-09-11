using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GoToNextMap : MonoBehaviour
{
    [SerializeField] string nextMapName;
    private bool canUnlock = false;
    [SerializeField] GameObject boss;
    UI ui;
    void Start()
    {
        ui = FindObjectOfType<UI>();
        EnemyStats enemyStats = boss.GetComponent<EnemyStats>();
        enemyStats.bossIsDead += UnlockNextMap;
    }

    void UnlockNextMap()
    {
        canUnlock = true;
        Debug.Log("Co the di den map moi");
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.GetComponent<Player>())
        {
            if(canUnlock)
                if(nextMapName != null)
                {
                    ui.SwitchOnWinScreen();
                    //SceneManager.LoadScene(nextMapName);
                }
                else
                {
                    Debug.Log("No more map");
                }
        }
    }
}
