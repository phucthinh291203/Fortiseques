using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class PopUpTextFx : MonoBehaviour
{
    private TextMeshPro myText;

    [SerializeField] private float speed;
    [SerializeField] private float disappearanceSpeed;
    [SerializeField] private float colorDisappearSpeed;

    [SerializeField] private float lifeTime;

    private float textTimer;
    // Start is called before the first frame update
    void Start()
    {
        myText = GetComponent<TextMeshPro>();
        textTimer = lifeTime;
    }

    // Update is called once per frame
    void Update()
    {
        transform.position = Vector2.MoveTowards(transform.position, new Vector2(transform.position.x, transform.position.y + 1), speed * Time.deltaTime);
        textTimer -= Time.deltaTime;

        if(textTimer < 0) {
            float alpha = myText.color.a - colorDisappearSpeed * Time.deltaTime;
            myText.color = new Color(myText.color.r, myText.color.g, myText.color.b, alpha);

            if (myText.color.a < 50)
                speed = disappearanceSpeed;

            if (myText.color.a < 0)
                Destroy(gameObject);
        }
        
    }
}
