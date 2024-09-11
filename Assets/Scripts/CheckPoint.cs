using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CheckPoint : MonoBehaviour
{
    private Animator anim;
    public string checkpointId;
    public bool activationStatus;
    private void Start()
    {
        anim = GetComponent<Animator>();
    }

    [ContextMenu("Generated checkpoint id")]
    public void GenerateId()
    {
        checkpointId = System.Guid.NewGuid().ToString();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.GetComponent<Player>() != null && activationStatus == false)
        {
            ActivateCheckpoint();
        }
    }

    public void ActivateCheckpoint()
    {
        if (activationStatus == false)
        {
            AudioManager.instance.PlaySFX(4, transform);
        }
        activationStatus = true;
        anim.SetBool("active", true);
    }
}
