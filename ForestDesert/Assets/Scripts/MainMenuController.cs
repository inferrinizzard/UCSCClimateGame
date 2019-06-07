using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class MainMenuController : MonoBehaviour
{
    public GameObject SettingsReference;
    public InfoController InfoReference;
    public GameObject NotificationReference;
    public Text MoneyText;
    public Text TurnText;
    public Text ActionText;

    private bool SettingsOn = false;
    private bool InfoOn = false;
    private bool NotificationsOn = false;
    private int BaseActions = 2;

    public static Dictionary<string, int> Scenes = new Dictionary<string, int>(){{"arctic",3},{"forest",1},{"city",4},{"tropics",2},{"desert",-1}};
    // Start is called before the first frame update
    void Start()
    {
        TurnText.text = "Turn " + GlobalStatics.Turn;
    }

    // Update is called once per frame
    void Update()
    {
        MoneyText.text = "Money: $" + string.Format("{0:0,0}", GlobalStatics.CashMoney);

        if(ActionText)
            ActionText.text = "Actions Remaining: " + GlobalStatics.ActionsRemaining;
    }

    public void IncrementTurn()
    {
        GlobalStatics.Turn++;
        TurnText.text = "Turn " + GlobalStatics.Turn;
        GlobalStatics.ActionsRemaining = BaseActions;
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
            // InfoReference.gameObject.SetActive(InfoOn);
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
