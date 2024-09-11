using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TextBtn : MonoBehaviour
{
    public void Reward()
    {
        InterstitialAdExample.instance.LoadAd_Rewarded((reward) =>
        {
            Debug.Log("Thanh cong");
            //Game tiep tuc chay
        });

        
    }

    public void Inter()
    {
        InterstitialAdExample.instance.LoadAd_Interstitial((reward) =>
        {
            Debug.Log("Thanh cong");
            //Game tiep tuc chay
        });


    }
}
