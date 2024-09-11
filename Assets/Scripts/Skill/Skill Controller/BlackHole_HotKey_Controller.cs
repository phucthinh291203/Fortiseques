using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class BlackHole_HotKey_Controller : MonoBehaviour
{
    private SpriteRenderer sr;
    private KeyCode myHotKey;
    private TextMeshProUGUI myText;

    private Transform myEnemy;
    private BlackHole_Skill_Controller blackHole;
    private bool touch = true;
    
    public void SetHotKey(KeyCode _myNewHotKey,Transform _myEnemy,BlackHole_Skill_Controller _myBlackHole)
        {
            sr = GetComponent<SpriteRenderer>();
            myText = GetComponentInChildren<TextMeshProUGUI>();

            //Gán dữ liệu riêng biệt cho từng hotKey được sinh ra
            myEnemy = _myEnemy;
            blackHole = _myBlackHole;

            myHotKey = _myNewHotKey;
            myText.text = _myNewHotKey.ToString();

        }

    public void Update()
    {
        if(touch == true) //Nếu bấm đúng hotKey nào thì hotKey đó sẽ biến mất
        {
            blackHole.AddEnemyToList(myEnemy); // goi blackhole luu transform cua con enemy tai hotkey nay vào 1 List
            myText.color = Color.clear;
            sr.color = Color.clear;
        }
    }

    public void TouchButton()
    {
        Debug.Log("Button clicked: " + myHotKey);
        touch = true;

        Debug.Log("Chạy");
    }
}
