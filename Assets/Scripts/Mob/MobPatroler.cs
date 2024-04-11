using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MobPatroler : MonoBehaviour
{
    public float speed;
    public float patrolingDistance;
    public Transform patrolingPoint;
    public float angerDistance;

    Status status;
    Transform playerTransform;
    bool movinRight = true;
    float patrolingDistanceDeltaY = 2;

    enum Status
    {
        PATROLING,
        AGGRESSIVE,
        GOING_BACK,
        DEAD,
        DESPAWNED
    };
    // Start is called before the first frame update
    void Start()
    {
        playerTransform = GameObject.FindGameObjectWithTag("Player").transform;
        status = Status.PATROLING;
    }

    // Update is called once per frame
    void Update()
    {
        calculateStatus();
        performAction();
    }

    private void calculateStatus()
    {
        if (status == Status.DEAD || status == Status.DESPAWNED)
        {
            return;
        }

        if (Vector2.Distance(playerTransform.position, transform.position) <= angerDistance)
        {
            status = Status.AGGRESSIVE;
        }
        else if (!isInPatrolingArea())
        {
            status = Status.GOING_BACK;
        }
        else
        {
            status = Status.PATROLING;
        }
    }

    
    private void performAction()
    {
        switch (status)
        {
            case Status.PATROLING:
                doPatrol();
                break;
            case Status.AGGRESSIVE:
                doFollowPlayer();
                break;
            case Status.GOING_BACK:
                doGoBack();
                break;
        }
    }

    private void doPatrol()
    {
        if (movinRight)
        {
            transform.Translate(Vector3.right * Time.deltaTime * speed);
        }
        else
        {
            transform.Translate(Vector3.left * Time.deltaTime * speed);
        }

        if (transform.position.x >= patrolingPoint.position.x + patrolingDistance)
        {
            movinRight = false;
        }
        else if (transform.position.x <= patrolingPoint.position.x - patrolingDistance)
        {
            movinRight = true;
        }
    }

    private bool isInPatrolingArea()
    {
        return Mathf.Abs(transform.position.x - patrolingPoint.position.x) <= patrolingDistance &&
            Mathf.Abs(transform.position.y - patrolingPoint.position.y) <= patrolingDistanceDeltaY;
    }

    private void doFollowPlayer()
    {
        transform.position = Vector2.MoveTowards(transform.position, playerTransform.position, speed * Time.deltaTime);
    }

    private void doGoBack()
    {
        transform.position = Vector2.MoveTowards(transform.position, patrolingPoint.position, speed * Time.deltaTime);
    }
}
