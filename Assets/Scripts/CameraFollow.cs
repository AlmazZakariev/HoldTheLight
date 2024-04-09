using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    public GameObject player;
    public float offsetY;
    public float check;
    // Start is called before the first frame update
    void Start()
    {
        SetOffet();
    }

    // Update is called once per frame
    void LateUpdate()
    {
        var position = transform.position;
        check = offsetY + player.transform.position.y;
        position.y = offsetY+player.transform.position.y;
        transform.position = position;

    }
    void SetOffet()
    {
        offsetY = transform.position.y;
    }
}
