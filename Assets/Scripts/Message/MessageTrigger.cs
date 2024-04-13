using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MessageTrigger : MonoBehaviour
{
    [TextArea(3, 10)]
    public string message;
    private bool triggerUsed = false;
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player")&& !triggerUsed)
        {
            triggerUsed = true;
            MessageManager.displayMessageEvent?.Invoke(message);
            
        }
    }
    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            
        }
    }
}
