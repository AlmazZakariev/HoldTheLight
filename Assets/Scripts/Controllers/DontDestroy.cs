using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DontDestroy : MonoBehaviour
{
    public GameObject backrgoundMusic;
    private AudioSource backrgoundaudioSrc;
    public GameObject[] existingAudioSources;

    void Awake()
    {
        existingAudioSources = GameObject.FindGameObjectsWithTag("Music");
        if (existingAudioSources.Length == 0)
        {
            backrgoundMusic = Instantiate(backrgoundMusic);
            backrgoundMusic.name = "BGMusic1";
            DontDestroyOnLoad(backrgoundMusic.gameObject);
        }
        else
        {
            backrgoundMusic = GameObject.Find("BGMusic1");
        }
    }
    void Start()
    {
        backrgoundaudioSrc = backrgoundMusic.GetComponent<AudioSource>();
    }
}
