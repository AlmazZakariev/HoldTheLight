using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PatrolingArea : MonoBehaviour
{
    public bool isPlayerInside;

    // Start is called before the first frame update
    void Start()
    {
        isPlayerInside = false;
        // make invisible *kolhoz*
        GetComponent<SpriteRenderer>().color = new Color(0, 0, 0, 0);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            isPlayerInside = true;
        }
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            isPlayerInside = false;
        }
    }
}
