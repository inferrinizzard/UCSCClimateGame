using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuController : MonoBehaviour
{
    public GameObject SettingsReference;
    public InfoController InfoReference;
    public GameObject NotificationReference;

    private bool SettingsOn = false;
    private bool InfoOn = false;
    private bool NotificationsOn = false;

    public static Dictionary<string, int> Scenes = new Dictionary<string, int>(){{"arctic",3},{"forest",1},{"city",4},{"tropics",2},{"desert",-1}};
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
        {
            InfoReference.gameObject.SetActive(InfoOn);
            InfoReference.bRenderOnNextFrame = true;
        }
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

    public void ReturnToMenu(){
        SceneManager.LoadScene(0);
    }
}
