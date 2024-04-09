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
    public float test = 0;

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
        test = currentLight / maxLight;
        lightBar.fillAmount = test;
        if (currentLight <= 0)
        {
            GameOver();
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
