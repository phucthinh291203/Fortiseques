using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AfterImage : MonoBehaviour
{
    private SpriteRenderer sr;
    private float colorLooseRate;

    public void SetupAfterImage(Sprite _spriteImage,float _loosingSpeed)
    {
        Debug.Log("vô được setup");
        sr = GetComponent<SpriteRenderer>();
        sr.sprite = _spriteImage;
        colorLooseRate = _loosingSpeed;

    }

    private void Update()
    {
        float alpha = sr.color.a - colorLooseRate * Time.deltaTime;
        sr.color = new Color(sr.color.r,sr.color.g,sr.color.b,alpha);

        if (sr.color.a <= 0)
            Destroy(gameObject);
    }
}
