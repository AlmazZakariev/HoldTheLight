using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using TMPro;
//using UnityEngine.SceneManagement;

public class MessageManager : MonoBehaviour
{
    public static Action<string> displayMessageEvent;
    public static Action disableMessageEvent;
    public TMP_Text messageText;

    public Animator animator;
    

    // Start is called before the first frame update
    void Start()
    {
        
    }
    private void OnEnable()
    {
        displayMessageEvent += DisplayMessage;
        disableMessageEvent += DisableMessage;
    }
    private void OnDisable()
    {
        displayMessageEvent -= DisplayMessage;
        disableMessageEvent -= DisableMessage;
    }
    private void DisplayMessage(string message)
    {       
        messageText.text = message; 
        animator.SetInteger("State", 1);
        Time.timeScale = 0;
    }
    private void DisableMessage()
    {
        animator.SetInteger("State", 0);
        Time.timeScale = 1;
    }
}
