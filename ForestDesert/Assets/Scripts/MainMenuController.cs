using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuController : MonoBehaviour
{
    public GameObject SettingsReference;
    public GameObject InfoReference;
    public GameObject NotificationReference;

    private bool SettingsOn = false;
    private bool InfoOn = false;
    private bool NotificationsOn = false;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void ToggleSettings()
    {
        SettingsOn = !SettingsOn;

        if (SettingsReference)
            SettingsReference.SetActive(SettingsOn);
    }

    public void ToggleInfo()
    {
        InfoOn = !InfoOn;

        if (InfoReference)
            InfoReference.SetActive(InfoOn);
    }

    public void ToggleNotifications()
    {
        NotificationsOn = !NotificationsOn;

        if (NotificationReference)
            NotificationReference.SetActive(NotificationsOn);
    }

    public void UpdateTemperature(string s)
    {
        GlobalStatics.Temperature = float.Parse(s);
    }

    public void ChangeLevel(string name)
    {
        SceneManager.LoadScene(name);
    }

    public void QuitGame()
    {
        Application.Quit();
    }
}
