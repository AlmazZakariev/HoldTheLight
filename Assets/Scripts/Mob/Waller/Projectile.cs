using UnityEngine;

public class Projectile : MonoBehaviour
{
    #region ��������� �������

    [SerializeField] private float speed = 4f; // �������� �������
    [SerializeField] private Vector3 direction; // ����������� ��������

    #endregion

    #region ������ Unity

    private void Start()
    {
        Destroy(gameObject, 5f); // ���������� ������ ����� 5 ������
    }

    private void Update()
    {
        transform.Translate(direction * speed * Time.deltaTime);
    }

    private void OnCollisionEnter2D(UnityEngine.Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            OnPlayerHit();
        }
    }

    #endregion

    #region ������ �������

    public void SetDirection(Vector3 direction)
    {
        this.direction = direction.normalized;
    }

    private void OnPlayerHit()
    {
        // ����� ����� ����������� ������ ��� ��������� � ������
        // ��������, ������� ����, ��������, ���������, etc.
        Debug.Log("player got hit");
        Destroy(gameObject);
    }

    #endregion
}
