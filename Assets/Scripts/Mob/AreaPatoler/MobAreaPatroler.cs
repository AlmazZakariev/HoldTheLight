using System.Collections;
using UnityEngine;

public class MobAreaPatroler : MonoBehaviour
{
    public GameObject patrolingArea;
    public float speed;

    Status status;
    Transform playerTransform;
    PatrolingAreaPointOperations patrolingAreaPointOperations;
    float minDistanceToPoint = 1;

    // Start is called before the first frame update
    void Start()
    {
        playerTransform = GameObject.FindGameObjectWithTag("Player").transform;
        status = Status.PATROLING;
        patrolingAreaPointOperations = new PatrolingAreaPointOperations(patrolingArea, StartCoroutine);
    }

    void Update()
    {
        calculateStatus();
        performAction();
    }

    private void calculateStatus()
    {
        switch (status)
        {
            case Status.PATROLING:
                if (isPlayerInsidePatrolingArea())
                    status = Status.AGGRESSIVE;
                break;
            case Status.AGGRESSIVE:
                if (!isPlayerInsidePatrolingArea())
                    status = Status.PATROLING;
                break;
        }
    }

    private void performAction()
    {
        Debug.Log(status);
        switch (status)
        {
            case Status.PATROLING:
                doPatrol();
                break;
            case Status.AGGRESSIVE:
                doFollowPlayer();
                break;
        }
    }

    private void doPatrol()
    {
        if (patrolingAreaPointOperations.isPointReady())
        {
            if (isPointReached())
            {
                patrolingAreaPointOperations.generateRandomPoint();
            }
            else
            {
                transform.position = Vector2.MoveTowards(transform.position,
                    patrolingAreaPointOperations.getCurrentPoint(), speed * Time.deltaTime);
            }
        }
    }

    private bool isPointReached()
    {
        var distanceToPoint = (patrolingAreaPointOperations.getCurrentPoint() - (Vector2)transform.position).magnitude;
        return distanceToPoint < minDistanceToPoint;
    }

    private void doFollowPlayer()
    {
        transform.position = Vector2.MoveTowards(transform.position, playerTransform.position, speed * Time.deltaTime);
    }

    private bool isPlayerInsidePatrolingArea()
    {
        return patrolingArea.GetComponent<PatrolingArea>().isPlayerInside;
    }
}

class PatrolingAreaPointOperations
{
    private GameObject patrolingArea;
    private System.Func<IEnumerator, Coroutine> startCoroutine;
    private Vector2 currentPoint { get; set; }
    private bool pointReady { get; set; }

    private int minDelayTimeSeconds = 1;
    private int maxDelayTimeSeconds = 5;
    System.Random random = new System.Random();

    public PatrolingAreaPointOperations(GameObject patrolingArea, System.Func<IEnumerator, Coroutine> startCoroutine)
    {
        this.patrolingArea = patrolingArea;
        this.startCoroutine = startCoroutine;
        startCoroutine(generateRandomPointDelayed(0));
    }

    public bool isPointReady()
    {
        return pointReady;
    }

    public Vector2 getCurrentPoint()
    {
        return currentPoint;
    }

    public void generateRandomPoint()
    {
        pointReady = false;
        startCoroutine(generateRandomPointDelayed(getRandomFloat(minDelayTimeSeconds, maxDelayTimeSeconds)));
    }

    public IEnumerator generateRandomPointDelayed(float delaySeconds)
    {
        yield return new WaitForSeconds(delaySeconds);
        var bounds = patrolingArea.GetComponent<BoxCollider2D>().bounds;
        currentPoint = new Vector2(
            bounds.center.x + getRandomFloat(-bounds.extents.x, bounds.extents.x),
            bounds.center.y + getRandomFloat(-bounds.extents.y, bounds.extents.y)
        );
        pointReady = true;
    }

    private float getRandomFloat(float min, float max)
    {
        double range = (double)max - (double)min;
        double sample = random.NextDouble();
        double scaled = (sample * range) + min;
        return (float)scaled;
    }
}
