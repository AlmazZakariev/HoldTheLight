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
    
    private void OnTriggerEnter2D(Collider2D collider)
    {
        // Для проверки попадания на платформу
    
        // Для проверки подбора батарейки
        if (collider.gameObject.CompareTag("Player"))
        {

            Destroy(gameObject);
            gameManager.AddLight(amountOfLight);
            
        }
    }
}
