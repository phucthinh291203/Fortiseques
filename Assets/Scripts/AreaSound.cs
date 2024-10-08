using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class AreaSound : MonoBehaviour
{
    [SerializeField] private int areaSoundIndex;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.GetComponent<Player>() != null)
        {
            AudioManager.instance.PlaySFX(areaSoundIndex, null);
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.GetComponent<Player>() != null)
            AudioManager.instance.StopSFXWithTime(areaSoundIndex);
    }

}
