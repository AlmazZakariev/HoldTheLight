using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEditor.Rendering.Universal;
using UnityEngine;
using UnityEngine.Rendering.Universal;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public int lvlNum;
    public float maxLight = 30f;
    public float currentLight;
    //соотношение текущего света к максимальному. 
    // используется для определения минимального размера
    public float amount0 = 0.5f;
    //используется для определения момента, когда фонарик начинает уменьшаться
    public float amount1= 0.5f;
    //используется для определения момента, когда фонарик перестаёт уменьшаться
    public float amount2 = 0.35f;
    //используется для определения момента, когда фонарик начнёт моргать
    public float amount3 = 0.1f;
    private bool lightBlink;
    private bool lightBlinkDoing = true;
    public float lightBlinkingPeriod = 1f;
    public float lightBlinkPower = 0.05f;
    public float scalePerSecond = 1f;
    
    private Light2D mainLightScript;
    
    public GameObject player;
    public GameObject backgroundImg;
    public float nextYPosition = 17f;
    public float bgSize = 17;
    public float newBGDist = 20;

    public Image lightBar;
    private PlayerController playerController;
    public GameObject textGameOver;
    public GameObject gameOverMenu;
    public bool gameOver = false;


    //light preset
    private float innerRadius;
    private float outerRadius;
    private float intensity;

    private CameraFollow cameraFollowScript;
    // Start is called before the first frame update
    void Start()
    {
        currentLight = maxLight;
        playerController = GameObject.Find("Player").GetComponent<PlayerController>();
        mainLightScript = GameObject.Find("MainLight").GetComponent<Light2D>();
        LightPreset();
        cameraFollowScript = GameObject.Find("Main Camera").GetComponent<CameraFollow>();
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
        if (player.transform.position.y - nextYPosition < newBGDist&&cameraFollowScript.VerticalScene)
        {
            var pos = backgroundImg.transform.position;
            pos.y = nextYPosition;
            Instantiate(backgroundImg, pos, backgroundImg.transform.rotation);
            nextYPosition -= bgSize;
        }
        ControlLigth();
        
    }
    private void FixedUpdate()
    {
        if (gameOver)
        {
            return;
        }
        if (lightBlink&& lightBlinkDoing)
        {
            mainLightScript.intensity -= lightBlinkPower;
            if (mainLightScript.intensity <= 0)
            {
                lightBlinkPower = -lightBlinkPower;
            }
            if (mainLightScript.intensity >= 1)
            {
                lightBlinkPower = -lightBlinkPower;
                lightBlinkDoing = false;
                Invoke("LightBlinkingPause", lightBlinkingPeriod);
            }
            
        
        }
    }
    private void LightBlinkingPause()
    {
        lightBlinkDoing = true;
    }
    private void ControlLigth()
    {
        //при lightPersent>= имеем радиусы 100%
        var lightPersent = currentLight / maxLight;
        if (lightPersent < amount1 && lightPersent > amount2)
        {
            mainLightScript.pointLightInnerRadius = innerRadius * (lightPersent + 1 - amount1)*amount0;
            mainLightScript.pointLightOuterRadius = outerRadius * (lightPersent + 1 - amount1)*amount0;
        }
        else if (lightPersent >= amount1)
        {
            mainLightScript.pointLightInnerRadius = innerRadius;
            mainLightScript.pointLightOuterRadius = outerRadius;
            mainLightScript.intensity = intensity;
        }
        if (lightPersent < amount3)
        {
            lightBlink = true;
        }
        else
        {
            lightBlink = false;
        }
    }
    private void LightPreset()
    {
        innerRadius = mainLightScript.pointLightInnerRadius;
        outerRadius= mainLightScript.pointLightOuterRadius;
        intensity = mainLightScript.intensity;
    }
    public bool AddLight(float value)
    {
        if (currentLight + value < 0)
        {
            return false;
        }
        currentLight+= value;
        if (currentLight> maxLight)
        {
            currentLight= maxLight;
        }
        return true;
    }
    public void GameOver()
    {
        gameOver = true;
        playerController.GameOver();
        gameOverMenu.SetActive(true);
    }
}
