using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BackgroundMove : MonoBehaviour
{
    public GameObject player;
    public GameObject backgroundImg;
    public float nextYPosition = -20.5f;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (player.transform.position.y - nextYPosition < 10)
        {
            var pos = backgroundImg.transform.position;
            pos.y = nextYPosition;
            Instantiate(backgroundImg, pos, backgroundImg.transform.rotation);
            nextYPosition += 20;
        }
    }
}
