using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    public string firstSceneName;
 
    public void Play()
    {
        SceneManager.LoadScene(firstSceneName);
    }
}
