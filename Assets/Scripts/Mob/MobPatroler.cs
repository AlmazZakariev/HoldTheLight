using UnityEngine;
using UnityEngine.AI;

public class MobPatroler : MonoBehaviour
{
    public float speed;
    public float patrolingDistance;
    public float angerDistance;
    public float maxTimeToFlyInOneDirection = 3f;

    Status status;
    Transform playerTransform;
    bool movinRight = true;
    float patrolingDistanceDeltaY = 2;
    Vector2 patrolingPoint;
    float nextRotationTime = 0f;

    NavMeshAgent agent;

    // Start is called before the first frame update
    void Start()
    {
        playerTransform = GameObject.FindGameObjectWithTag("Player").transform;
        status = Status.PATROLING;
        patrolingPoint = transform.position;
        agent = GetComponent<NavMeshAgent>();
        agent.updateRotation = false;
        agent.updateUpAxis = false;
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
            //transform.Translate(Vector3.right * Time.deltaTime * speed);
            move(transform.position + Vector3.right);
        }
        else
        {
            //transform.Translate(Vector3.left * Time.deltaTime * speed);
            move(transform.position + Vector3.left);
        }

        if (transform.position.x >= patrolingPoint.x + patrolingDistance)
        {
            movinRight = false;
            nextRotationTime = Time.time + maxTimeToFlyInOneDirection;
        }
        else if (transform.position.x <= patrolingPoint.x - patrolingDistance)
        {
            movinRight = true;
            nextRotationTime = Time.time + maxTimeToFlyInOneDirection;
        }
        else if (Time.time >= nextRotationTime)
        {
            movinRight = !movinRight;
            nextRotationTime = Time.time + maxTimeToFlyInOneDirection;
        }
    }

    private bool isInPatrolingArea()
    {
        return Mathf.Abs(transform.position.x - patrolingPoint.x) <= patrolingDistance &&
            Mathf.Abs(transform.position.y - patrolingPoint.y) <= patrolingDistanceDeltaY;
    }

    private void doFollowPlayer()
    {
        //transform.position = Vector2.MoveTowards(transform.position, playerTransform.position, speed * Time.deltaTime);
        move(playerTransform.position);
        movinRight = playerTransform.position.x - transform.position.x > 0;
    }

    private void doGoBack()
    {
        //transform.position = Vector2.MoveTowards(transform.position, patrolingPoint, speed * Time.deltaTime);
        move(patrolingPoint);
        movinRight = patrolingPoint.x - transform.position.x > 0;
    }

    private void move(Vector2 pos)
    {
        if (agent.isActiveAndEnabled && agent.isOnNavMesh)
        {
            agent.SetDestination(pos);
        }
        else
        {
            transform.position = Vector2.MoveTowards(transform.position, pos, speed * Time.deltaTime);
        }

        this.pos = pos;
    }
    Vector2 pos = Vector2.down;
    public void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(pos, 0.5f);
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(patrolingPoint, patrolingDistance);
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(transform.position, 1f);
    }
}
