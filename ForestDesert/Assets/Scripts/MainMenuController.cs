using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class MainMenuController : MonoBehaviour
{
    // Reference to the settings UI canvas group
    public GameObject SettingsReference;
    // Reference to the info UI canvas group
    public InfoController InfoReference;
    // Reference to the notification UI canvas group
    public GameObject NotificationReference;

    // Reference to the money text display
    public Text MoneyText;
    // Reference to the current turn number display
    public Text TurnText;
    // Reference to the amount of actions left display
    public Text ActionText;

    // Bools containing whether or not each UI group is active
    private bool SettingsOn = false;
    private bool InfoOn = false;
    private bool NotificationsOn = false;

    // Amount of actions you'll get at the start of a turn
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

    // Increments the turn by one. Resets actions remaining
    public void IncrementTurn()
    {
        GlobalStatics.Turn++;
        TurnText.text = "Turn " + GlobalStatics.Turn;
        GlobalStatics.ActionsRemaining = BaseActions;
    }

    // Turns the settings canvas group on or off
    public void ToggleSettings()
    {
        SettingsOn = !SettingsOn;

        if (SettingsReference)
            SettingsReference.SetActive(SettingsOn);
    }

    // Turns the info canvas group on or off
    public void ToggleInfo()
    {
        InfoOn = !InfoOn;

        if (InfoReference)
        {
            // InfoReference.gameObject.SetActive(InfoOn);
            InfoReference.bRenderOnNextFrame = true;
        }
    }

    // Turns the notifications canvas group on or off
    public void ToggleNotifications()
    {
        NotificationsOn = !NotificationsOn;

        if (NotificationReference)
            NotificationReference.SetActive(NotificationsOn);
    }

    // Sets the temperature by parsing the input string
    public void UpdateTemperature(string s)
    {
        GlobalStatics.Temperature = float.Parse(s);
    }

    // Sets the base actions by parsing the input string
    public void UpdateActionAmount(string s)
    {
        GlobalStatics.ActionsRemaining = int.Parse(s);
        BaseActions = int.Parse(s);
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
