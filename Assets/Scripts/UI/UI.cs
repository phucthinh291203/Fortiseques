using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;

public class UI : MonoBehaviour,ISaveManager
{
    [Header("End screen")]
    [SerializeField] private UI_FadeScreen fadeScreen;
    [SerializeField] private GameObject endText;
    [SerializeField] private GameObject restartButton;

    [Header("Win screen")]
    [SerializeField] private GameObject winUI;
    [SerializeField] private GameObject winText;
    [SerializeField] private GameObject messageText;
    [SerializeField] private GameObject backToMenuButton;
    [Space]
    [SerializeField] private GameObject characterUI;
    [SerializeField] private GameObject skillTreeUI;
    [SerializeField] private GameObject craftUI;
    [SerializeField] private GameObject optionUI;
    [SerializeField] private GameObject inGameUI;
    [SerializeField] private GameObject shopUI;

    [Header("For Ads")]
    [SerializeField] private GameObject adsUI;
    [SerializeField] private int soulsAmount;

    [Header("Souls currency")]
    [SerializeField] private TextMeshProUGUI playerCurrencyUI;
    [SerializeField] private TextMeshProUGUI playerCurrencyInSkillUI;
    public UI_ItemToolTip itemToolTip;
    public UI_StatToolTip statToolTip;
    public UI_SkillToolTip skillToolTip;
    public UI_CraftWindow craftWindow;
    public UI_ShopWindow shopWindow;

    [SerializeField] private UI_VolumeSlider[] volumeSetting;
    
    public event Action ResetText;

    private void Awake()
    {

        SwitchTo(skillTreeUI);
        fadeScreen.gameObject.SetActive(true);
        //Bật cái này trước tránh lỗi unlock skill slot mà không dùng skill được
        //Bật lên nhanh xong chuyển qua start sẽ ẩn đi -> Button Skill tree sẽ chạy hàm awake trước
    }

    void Start()
    {
        SwitchTo(inGameUI);
        itemToolTip.gameObject.SetActive(false);
        statToolTip.gameObject.SetActive(false);
        skillToolTip.gameObject.SetActive(false);
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.C))
        {
            OpenCharacterUI();
        }
        if (Input.GetKeyDown(KeyCode.B))
        {
            OpenCraftUI();
        }
        if (Input.GetKeyDown(KeyCode.K))
        {
            OpenSkillTreeUI();
        }
        if (Input.GetKeyDown(KeyCode.U))
        {
            OpenOptionUI();
        }
    }

    public void OpenSkillTreeUI()
    {
        SwitchWitKeyTo(skillTreeUI);
    }

    public void OpenCraftUI()
    {
        SwitchWitKeyTo(craftUI);
    }

    public void OpenCharacterUI()
    {
        SwitchWitKeyTo(characterUI);
    }

    public void OpenOptionUI()
    {
        SwitchWitKeyTo(optionUI);
    }

    public void OpenShopUI()
    {
        SwitchWitKeyTo(shopUI);
        ResetText();
    }

    public void OpenAdsUI()
    {
        SwitchWitKeyTo(adsUI);
        playerCurrencyUI.text = "Your souls: " + PlayerManager.instance.GetCurrentCurrency().ToString();
    }

    public void SwitchTo(GameObject _menu)
    {
        for(int i=0;i< transform.childCount;i++)
        {
            bool isFadeScreen = transform.GetChild(i).GetComponent<UI_FadeScreen>() != null;
            if ((isFadeScreen) == false)    //Tắt hết ngoại trừ fadeScreen
                transform.GetChild(i).gameObject.SetActive(false);  //Tắt hết những thứ trong canvas đi 
        }
        if (_menu != null)
        {   
            if(AudioManager.instance != null)   //Audio khởi tạo sau UI
                AudioManager.instance.PlaySFX(6, null);
            _menu.SetActive(true);          //Bật cái _menu mình chọn
        }
        if(_menu == skillTreeUI)
            playerCurrencyInSkillUI.text = "Your souls: " + PlayerManager.instance.GetCurrentCurrency().ToString();

        if (GameManager.instance != null)
        {
            if (_menu == inGameUI)
                GameManager.instance.PauseGame(false);
            else
                GameManager.instance.PauseGame(true);
        }    
    }

    public void SwitchWitKeyTo(GameObject _menu)
    {
        if(_menu != null && _menu.activeSelf)           //Bấm thêm 1 lần nữa để tắt (vd: tắt bảng skill)
        {
            _menu.SetActive(false);                     //Tắt bảng hiện tại -> quay về màn hình game
            CheckForInGameUI();                         
            return;
        }
        SwitchTo(_menu);                            //Bật lên
    }

    public void CheckForInGameUI()
    {
        for(int i= 0;i< transform.childCount; i++)
        {
            //menu đang bật và không phải là fadeScreen (fadeScreen luôn được bật)
            if (transform.GetChild(i).gameObject.activeSelf && transform.GetChild(i).GetComponent<UI_FadeScreen>() == null)
                return;
        }

        SwitchTo(inGameUI);  //Bật thanh máu, skill lên
    }

    public void SwitchOnEndScreen()
    {
        SwitchTo(null);
        GameManager.instance.PauseGame(false);
        fadeScreen.FadeOut();
        StartCoroutine(EndScreenCoroutine());
        
    }

    public void SwitchOnWinScreen()
    {
        SwitchTo(null);
        GameManager.instance.PauseGame(false);
        fadeScreen.FadeOut();
        StartCoroutine(WinScreenCoroutine());

    }

    IEnumerator EndScreenCoroutine()
    {
        yield return new WaitForSeconds(1.5f);
        endText.SetActive(true);
        yield return new WaitForSeconds(1.5f);
        restartButton.SetActive(true);
        Interstitial_Ad_PopUp();
    }

    IEnumerator WinScreenCoroutine()
    {
        winUI.SetActive(true);
        yield return new WaitForSeconds(1.5f);
        winText.SetActive(true);
        yield return new WaitForSeconds(1.5f);
        messageText.SetActive(true);
        yield return new WaitForSeconds(1.5f);
        backToMenuButton.SetActive(true);
        Interstitial_Ad_PopUp();
    }

    public void RestartGameButton() => GameManager.instance.RestartScene();

    public void SaveAndQuitButton() => GameManager.instance.SaveAndQuit();
    

    public void LoadData(GameData _data)
    {
        foreach(KeyValuePair<string,float> pair in _data.volumeSetting)
        {
            foreach(UI_VolumeSlider item in volumeSetting)
            {
                if (item.parameter == pair.Key)
                    item.LoadSlider(pair.Value);
            }
        }
    }

    public void SaveData(ref GameData _data)
    {
        _data.volumeSetting.Clear();
        foreach (UI_VolumeSlider item in volumeSetting)
            _data.volumeSetting.Add(item.parameter, item.slider.value);
    }


    public void Interstitial_Ad_PopUp()
    {
        InterstitialAdExample.instance.LoadAd_Interstitial((reward) =>
        {
            Debug.Log("Thanh cong");
            //Game tiep tuc chay
        });
    }

    public void Reward_Ad_PopUp()
    {
        InterstitialAdExample.instance.LoadAd_Rewarded((reward) =>
        {
            Debug.Log("Thanh cong");
            PlayerManager.instance.IncreaseCurrency(soulsAmount);
            playerCurrencyUI.text = "Your souls: " + PlayerManager.instance.GetCurrentCurrency().ToString();
        });
    }
}
