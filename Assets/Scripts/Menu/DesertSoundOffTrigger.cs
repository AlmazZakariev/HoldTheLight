using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class DesertSoundOffTrigger : MonoBehaviour
{
    //public AudioSource backGroundSound;
    public AudioSource desertSound;
    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            desertSound.Pause();
            //backGroundSound.Play();
        }
    }
}
