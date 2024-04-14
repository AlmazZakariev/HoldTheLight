using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LvlTransition : MonoBehaviour
{
    public int sceneNum;
    public GameObject panel;
    public void ChangeScene(int scene)
    {
        SceneManager.LoadScene(scene);
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            if (!panel.activeSelf)
            {
                panel.SetActive(true);
                //ChangeScene(sceneNum);
            }


        }
    }
}
