using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    [SerializeField]
    private GameObject player;
    [SerializeField]
    public bool VerticalScene;
    [SerializeField]
    private float offsetY  = 2.94f;
    [SerializeField]
    private Vector3 offset;
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
            //на вертикальном уровне камера следует только по оси Y
            position.y = offset.y + player.transform.position.y;        
        }
        else
        {
            //на горизонтальном уровне камера следует только по оси X
            position.x = -offset.x + player.transform.position.x;
        }
        transform.position = position;

    }
    void SetOffset()
    {      
        offset =  transform.position + player.transform.position;
        //offset.y = offsetY;
    }
}
