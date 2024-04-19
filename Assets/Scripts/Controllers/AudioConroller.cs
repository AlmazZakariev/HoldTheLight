using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioConroller : MonoBehaviour
{
    public AudioSource jumpSound;
    public AudioSource pickUpSound;
    public AudioSource batdeadSound;
    public void PlayJumpSound()
    {
        jumpSound.Play();
    }
    public void PlayPickUpSound()
    {
        pickUpSound.Play();
    }
    public void PlayBatDeadSound()
    {
        batdeadSound.Play();
    }
}
