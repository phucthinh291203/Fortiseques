using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParallaxCamera : MonoBehaviour
{
    private GameObject cam;
    [SerializeField] float parallaxEffect;
    private float xPosition;
    private float yPosition;
    private float length;
    void Start()
    {
        cam = GameObject.Find("Main Camera");
        xPosition = transform.position.x;
        yPosition = transform.position.y;
        length = GetComponent<SpriteRenderer>().bounds.size.x; 
    }

    // Update is called once per frame
    void Update()
    {
        float distanceMove = cam.transform.position.x * (1 - parallaxEffect);
        float distanceToMove = cam.transform.position.x * parallaxEffect;
        float yDistanceMove = cam.transform.position.y * (1 - parallaxEffect);
        float yDistanceToMove = cam.transform.position.y * parallaxEffect;
        transform.position = new Vector3(xPosition + distanceToMove, yPosition + yDistanceToMove);
        //Danh cho layer city (endless)
        if(distanceMove>xPosition + length) // vi tri camera vuot qua vi tri cua background va goc cua no
            xPosition = xPosition + length;
        if(distanceMove < xPosition - length)
            xPosition = xPosition - length;
    }
}
