using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShockStrike_Controller : MonoBehaviour
{
    [SerializeField] private CharacterStats targetStats;
    [SerializeField] private float speed;
    private int damage;
    private Animator anim;
    private bool triggered;
    void Start()
    {
        anim = GetComponentInChildren<Animator>();
    }

    public void SetUp(int _damage,CharacterStats _targetStats)
    {
        damage = _damage;
        targetStats = _targetStats;
    }
    // Update is called once per frame
    void Update()
    {
        if (!targetStats)
            return;

        if(triggered)
            return;
        
        transform.position = Vector2.MoveTowards(transform.position, targetStats.transform.position, speed * Time.deltaTime);
        transform.right = transform.position - targetStats.transform.position;
        if(Vector2.Distance(transform.position, targetStats.transform.position) < .1f)  
        {
            anim.transform.localRotation = Quaternion.identity; //quay child cua object thunder xuong dat
            anim.transform.localPosition = new Vector3(0, 0.5f); // sam set dich len tren mat dat 1 ti

            transform.localRotation = Quaternion.identity; 
            transform.localScale = new Vector3(3, 3); //Phong to hinh anh set danh xuong dat


            Invoke("DamageAndDestroy", .2f);
            triggered = true;
            anim.SetTrigger("Hit");
            
        }
    }

    public void DamageAndDestroy()
    {
        targetStats.ApplyShock(true);   //hieu ung fx
        targetStats.TakeDamage(damage);
        Destroy(gameObject, .4f);
    }
}
