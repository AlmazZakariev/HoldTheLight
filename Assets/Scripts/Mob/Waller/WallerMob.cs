using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;

public class MobController : MonoBehaviour
{
    #region ��������� ����

    [SerializeField] private float patrolSpeed = 2f; // �������� ��������������
    [SerializeField] private float patrolRange = 5f; // �������� ��������������
    [SerializeField] private float triggerDistance = 3f; // ���������� ������������ �����
    [SerializeField] private GameObject projectilePrefab; // ������ �������
    [SerializeField] private float fireRate = 1f; // �������� ��������
    [SerializeField] private float minPatrolDelay = 0f;
    [SerializeField] private float maxPatrolDelay = 3f;
    [SerializeField] private float patrolEpsilon = 1f;

    private Transform playerTransform; // ������ �� ������������� ������
    private float spawnPositionY;

    private float nextFireTime = 0f;
    private float nextPatrolTime = 0f;
    private float lastPatrolPointY;
    private System.Random random = new System.Random();

    #endregion

    #region ������ Unity

    private void Start()
    {
        playerTransform = GameObject.FindGameObjectWithTag("Player").transform;
        spawnPositionY = transform.position.y;
        lastPatrolPointY = spawnPositionY;
    }

    private void Update()
    {
        if (canAttackPlayer())
        {
            Attack();
        }
        else
        {
            Patrol();
        }
    }

    #endregion

    #region ������ ����

    private void Patrol()
    {
        // ���� ����� � ���� �������������� - ��� ���������� ���
        if (Mathf.Abs(playerTransform.position.y - spawnPositionY) < patrolRange)
        {
            var dir = new Vector2(0, playerTransform.position.y - transform.position.y).normalized;
            transform.Translate(dir * patrolSpeed * Time.deltaTime);
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
                var dir = new Vector2(0, lastPatrolPointY - transform.position.y).normalized;
                transform.Translate(dir * patrolSpeed * Time.deltaTime);
            }
        }
    }

    private bool canAttackPlayer()
    {
        return Mathf.Abs(playerTransform.position.y - transform.position.y) <= triggerDistance;
    }

    private void Attack()
    {
        if (Time.time >= nextFireTime)
        {
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
