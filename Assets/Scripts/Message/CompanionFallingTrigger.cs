using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CompanionFallingTrigger : MonoBehaviour
{
    public Rigidbody2D companionRb;
    // Start is called before the first frame update
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {

            {
                companionRb.gravityScale = 1;
            }
        }
    }
}

            
