using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public class MobAreaPatroler : MonoBehaviour
{
    public GameObject patrolingArea;
    public float speed;
    public Animator animator;
    public float maxPointTime = 5;

    Status status;
    Transform playerTransform;
    PatrolingAreaPointOperations patrolingAreaPointOperations;
    float minDistanceToPoint = 1;
    float nextPointTime = 0f;

    NavMeshAgent agent;

    #region ATTACK PARAMS
    public float attackDelay = 2f;
    public float attackDuration = 2f;
    public float attackRange = 2f;
    public float attackCancelRange = 2f;

    bool isAttacking;
    float nextAttackTime = 0f;
    float attackFinishTime = 0f;
    #endregion

    // Start is called before the first frame update
    void Start()
    {
        playerTransform = GameObject.FindGameObjectWithTag("Player").transform;
        status = Status.PATROLING;
        patrolingAreaPointOperations = new PatrolingAreaPointOperations(patrolingArea, StartCoroutine);
        agent = GetComponent<NavMeshAgent>();
        agent.updateRotation = false;
        agent.updateUpAxis = false;
    }

    void Update()
    {
        animator.SetFloat("HorizontalMove", 0);
        if (isAttacking)
        {
            turnFaceInPointDirection(playerTransform.position.x);
            if (getDistanceToPlayer() >= attackCancelRange)
            {
                cancelAttack();
            }
            else if (Time.time >= attackFinishTime)
            {
                finishAttack();
            }
            return;
        }

        bool isFollowingPlayer = isPlayerInsidePatrolingArea();
        if (isFollowingPlayer && getDistanceToPlayer() <= attackRange)
        {
            if (Time.time >= nextAttackTime)
            {
                startAttack();
            }
        }
        else if (isFollowingPlayer)
        {
            doFollowPlayer();
        }
        else
        {
            doPatrol();
        }
    }

    private void startAttack()
    {
        animator.SetTrigger("Attack");
        isAttacking = true;
        nextAttackTime = Time.time + attackDelay;
        attackFinishTime = Time.time + attackDuration;
    }

    private void finishAttack()
    {
        isAttacking = false;

        // player reaction to attack
        if (GameObject.FindGameObjectWithTag("Player")!=null)
        {
            var playerController = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerController>();
            playerController.onAttacked();
        }
        
    }

    private void cancelAttack()
    {
        isAttacking = false;
        nextAttackTime = 0f;
    }

    private float getDistanceToPlayer()
    {
        return (playerTransform.position - transform.position).magnitude;
    }

    private void doPatrol()
    {
        if (patrolingAreaPointOperations.isPointReady())
        {
            if (isPointReached() || Time.time >= nextPointTime)
            {
                patrolingAreaPointOperations.generateRandomPoint();
                nextPointTime = Time.time + maxPointTime;
            }
            else
            {
                //turnFaceInPointDirection(patrolingAreaPointOperations.getCurrentPoint().x);
                //animator.SetFloat("HorizontalMove", Mathf.Abs(transform.position.x - patrolingAreaPointOperations.getCurrentPoint().x) * speed * Time.deltaTime);
                //transform.position = Vector2.MoveTowards(transform.position,
                //    patrolingAreaPointOperations.getCurrentPoint(), speed * Time.deltaTime);

                move(patrolingAreaPointOperations.getCurrentPoint());
            }
        }
    }

    private void turnFaceInPointDirection(float pointX)
    {
        var localScaleModifier = (-transform.position.x + pointX) * transform.localScale.x < 0 ? -1 : 1;
        transform.localScale = new Vector3(transform.localScale.x * localScaleModifier, transform.localScale.y, transform.localScale.z);
    }

    private bool isPointReached()
    {
        var distanceToPoint = (patrolingAreaPointOperations.getCurrentPoint() - (Vector2)transform.position).magnitude;
        return distanceToPoint < minDistanceToPoint;
    }

    private void doFollowPlayer()
    {
        //turnFaceInPointDirection(playerTransform.position.x);
        //animator.SetFloat("HorizontalMove", Mathf.Abs((playerTransform.position - transform.position).x) * speed * Time.deltaTime);
        //transform.position = Vector2.MoveTowards(transform.position, playerTransform.position, speed * Time.deltaTime);

        move(playerTransform.position);
        
    }

    private void move(Vector2 pos) {
        turnFaceInPointDirection(pos.x);
        animator.SetFloat("HorizontalMove", Mathf.Abs((pos - (Vector2)transform.position).x) * speed * Time.deltaTime);

        if (agent.isActiveAndEnabled && agent.isOnNavMesh)
        {
            agent.SetDestination(pos);
        } 
        else
        {
            transform.position = Vector2.MoveTowards(transform.position, pos, speed * Time.deltaTime);
        }
    }

    private bool isPlayerInsidePatrolingArea()
    {
        return patrolingArea.GetComponent<PatrolingArea>().isPlayerInside;
    }

    private void OnDrawGizmosSelected()
    {
        var t = Gizmos.color;

        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange);

        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(transform.position, attackCancelRange);

        Gizmos.color = t;
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
