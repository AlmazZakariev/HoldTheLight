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
    public bool messageActive = false;

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
        messageActive= true;
        messageText.text = message; 
        animator.SetInteger("State", 1);
        

    }
    private void DisableMessage()
    {
        animator.SetInteger("State", 0);
        messageActive = false;
    }
}
