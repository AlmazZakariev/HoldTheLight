using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;

public class MobController : MonoBehaviour
{
    #region Параметры моба

    [SerializeField] private float patrolSpeed = 2f; // Скорость патрулирования
    [SerializeField] private float patrolRange = 5f; // Диапазон патрулирования
    [SerializeField] private float triggerDistance = 3f; // Расстояние срабатывания атаки
    [SerializeField] private GameObject projectilePrefab; // Префаб снаряда
    [SerializeField] private float fireRate = 1f; // Скорость стрельбы
    [SerializeField] private float minPatrolDelay = 0f;
    [SerializeField] private float maxPatrolDelay = 3f;
    [SerializeField] private float patrolEpsilon = 1f;
    [SerializeField] private Animator animator;

    private Transform playerTransform; // Ссылка на трансформацию игрока
    private float spawnPositionY;

    private float nextFireTime = 0f;
    private float nextPatrolTime = 0f;
    private float lastPatrolPointY;
    private System.Random random = new System.Random();

    #endregion

    #region Методы Unity

    private void Start()
    {
        playerTransform = GameObject.FindGameObjectWithTag("Player").transform;
        spawnPositionY = transform.position.y;
        lastPatrolPointY = spawnPositionY;
    }

    private void Update()
    {
        animator.SetBool("Running", false);
        var oldRot = transform.localRotation.z;
        var t = transform.localRotation;
        t.z = 0;
        transform.localRotation = t; 
        if (canAttackPlayer())
        {
            Attack();
        }
        else
        {
            Patrol();
        }
        t.z = oldRot;
        transform.localRotation = t;
    }

    #endregion

    #region Логика моба

    private void Patrol()
    {
        // если игрок в зоне патрулирования - моб преследует его
        if (Mathf.Abs(playerTransform.position.y - spawnPositionY) < patrolRange)
        {
            moveToY(playerTransform.position.y);
        }
        else if (Time.time >= nextPatrolTime)
        {
            if (Mathf.Abs(lastPatrolPointY - transform.position.y) < patrolEpsilon)
            {
                // gen new lastPatrolPointY
                lastPatrolPointY = getRandomFloat(spawnPositionY - patrolRange, spawnPositionY + patrolRange);
                nextPatrolTime = Time.time + getRandomFloat(minPatrolDelay, maxPatrolDelay);
            }
            else
            {
                moveToY(lastPatrolPointY);
            }
        }
    }

    private void moveToY(float yDestination)
    {
        var dir = new Vector2(0, yDestination - transform.position.y).normalized;

        //anim
        animator.SetBool("Running", true);
        flipToY(dir.y);

        transform.Translate(dir * patrolSpeed * Time.deltaTime);
    }

    private void flipToY(float dirY)
    {
        // if signs equals, flip
        if (dirY * transform.localScale.x > 0)
        {
            var t = transform.localScale;
            t.x *= -1;
            transform.localScale = t;
        }
    }

    private bool canAttackPlayer()
    {
        return Mathf.Abs(playerTransform.position.y - transform.position.y) <= triggerDistance;
    }

    private void Attack()
    {
        flipToY(playerTransform.position.y - transform.position.y);
        if (Time.time >= nextFireTime)
        {
            animator.SetTrigger("Attack");
            GameObject projectile = Instantiate(projectilePrefab, transform.position, Quaternion.identity);
            projectile.GetComponent<Projectile>().SetDirection(playerTransform.position - transform.position);

            nextFireTime = Time.time + fireRate;
        }
    }

    private float getRandomFloat(float min, float max)
    {
        double range = (double)max - (double)min;
        double sample = random.NextDouble();
        double scaled = (sample * range) + min;
        return (float)scaled;
    }

    #endregion
}
