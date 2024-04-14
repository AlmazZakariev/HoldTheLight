using UnityEngine;

public class Projectile : MonoBehaviour
{
    #region Параметры снаряда

    [SerializeField] private float speed = 4f; // Скорость снаряда
    [SerializeField] private Vector3 direction; // Направление движения

    #endregion

    #region Методы Unity

    private void Start()
    {
        Destroy(gameObject, 5f); // Уничтожаем снаряд через 5 секунд
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

    #region Логика снаряда

    public void SetDirection(Vector3 direction)
    {
        this.direction = direction.normalized;
    }

    private void OnPlayerHit()
    {
        // Здесь можно реализовать логику при попадании в игрока
        // Например, нанести урон, оглушить, замедлить, etc.
        Debug.Log("player got hit");
        Destroy(gameObject);
    }

    #endregion
}
