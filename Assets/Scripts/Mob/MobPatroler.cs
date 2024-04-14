using UnityEngine;

public class MobPatroler : MonoBehaviour
{
    public float speed;
    public float patrolingDistance;
    public float angerDistance;

    Status status;
    Transform playerTransform;
    bool movinRight = true;
    float patrolingDistanceDeltaY = 2;
    Vector2 patrolingPoint;

    // Start is called before the first frame update
    void Start()
    {
        playerTransform = GameObject.FindGameObjectWithTag("Player").transform;
        status = Status.PATROLING;
        patrolingPoint = transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        calculateStatus();
        performAction();
        FlipIfNeeded();
    }

    private void FlipIfNeeded()
    {
        if (transform.localScale.x > 0 && movinRight ||
            transform.localScale.x < 0 && !movinRight)
        {
            var t = transform.localScale;
            t.x *= -1;
            transform.localScale = t;
        }
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

        if (transform.position.x >= patrolingPoint.x + patrolingDistance)
        {
            movinRight = false;
        }
        else if (transform.position.x <= patrolingPoint.x - patrolingDistance)
        {
            movinRight = true;
        }
    }

    private bool isInPatrolingArea()
    {
        return Mathf.Abs(transform.position.x - patrolingPoint.x) <= patrolingDistance &&
            Mathf.Abs(transform.position.y - patrolingPoint.y) <= patrolingDistanceDeltaY;
    }

    private void doFollowPlayer()
    {
        transform.position = Vector2.MoveTowards(transform.position, playerTransform.position, speed * Time.deltaTime);
        movinRight = playerTransform.position.x - transform.position.x > 0;
    }

    private void doGoBack()
    {
        transform.position = Vector2.MoveTowards(transform.position, patrolingPoint, speed * Time.deltaTime);
    }
}
