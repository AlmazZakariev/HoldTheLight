using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameOverMenu : MonoBehaviour
{
    public string currentLevelName;

    public static void Restart()
    {
        PlayerPrefs.SetString("lastCheckpoint", null);
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public static void StartFromCp()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void LoadMenu()
    {
        Time.timeScale = 1;
        SceneManager.LoadScene("MainMenu");
    }
}
