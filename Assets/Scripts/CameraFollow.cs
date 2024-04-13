using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    public GameObject player;
    private Vector3 offset;
    public bool VerticalScene;
    // Start is called before the first frame update
    void Start()
    {
        SetOffset();
    }

    // Update is called once per frame
    void LateUpdate()
    {
        var position = transform.position;
        if (VerticalScene)
        { 
            position.y = offset.y + player.transform.position.y;
            
        }
        else
        {
            position.x = offset.x + player.transform.position.x;
        }
        transform.position = position;

    }
    void SetOffset()
    {      
        offset = transform.position;
    }
}
