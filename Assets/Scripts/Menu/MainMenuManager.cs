using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MainMenuManager : MonoBehaviour
{
    public GameObject showEducationToggleGO;
    public bool test;
    public static bool ShowEducation = true;


    private void Awake()
    {
        var showEducationTugle= showEducationToggleGO.GetComponent<Toggle>();
        showEducationTugle.isOn = ShowEducation;
    }
    private void Start()
    {
        Screen.SetResolution(607, 1080, false);
        var showEducationTugle = showEducationToggleGO.GetComponent<Toggle>();
        ShowEducation = showEducationTugle.isOn;
    }
    public void SetShowEducation()
    {
        ShowEducation = !ShowEducation;
    }
    private void Update()
    {
        test = ShowEducation;
    }
}
