using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BattaryController : MonoBehaviour
{
    GameManager gameManager;
    public float amountOfLight;
    // Start is called before the first frame update
    void Start()
    {
        gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    private void OnCollisionEnter2D(Collision2D collision)
    {
        // Для проверки попадания на платформу
    
        // Для проверки подбора батарейки
        if (collision.gameObject.CompareTag("Player"))
        {
            Destroy(gameObject);
            gameManager.AddLight(amountOfLight);
        }
    }
}
