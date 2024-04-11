using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public float maxLight = 30f;
    public float currentLight;
    public float scalePerSecond = 1f;
    
    public GameObject player;
    public GameObject backgroundImg;
    public float nextYPosition = 17f;
    public float bgSize = 17;
    public float newBGDist = 20;

    public Image lightBar;
    private PlayerController playerController;
    public GameObject textGameOver;
    private bool gameOver = false;
    // Start is called before the first frame update
    void Start()
    {
        currentLight = maxLight;
        playerController = GameObject.Find("Player").GetComponent<PlayerController>();
    }

    // Update is called once per frame
    void Update()
    {
        if (gameOver)
        {
            return;
        }
        currentLight -= scalePerSecond*Time.deltaTime;
        lightBar.fillAmount = currentLight / maxLight;
        if (currentLight <= 0)
        {
            GameOver();
        }

        //background control
        if (player.transform.position.y - nextYPosition < newBGDist)
        {
            var pos = backgroundImg.transform.position;
            pos.y = nextYPosition;
            Instantiate(backgroundImg, pos, backgroundImg.transform.rotation);
            nextYPosition -= bgSize;
        }
    }
    public bool AddLight(float value)
    {
        if (currentLight + value < 0)
        {
            return false;
        }
        currentLight+= value;
        return true;
    }
    public void GameOver()
    {
        gameOver = true;
        playerController.GameOver();
        textGameOver.SetActive(true);
    }
}
