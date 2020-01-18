using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIController : MonoBehaviour {
    public GameObject settingsGroup;
    public InfoController infoGroup;
    public GameObject notificationGroup;
    public Text moneyText;
    public Text turnText;
    public Text actionText;

    bool settingsOn = false;
    bool infoOn = false;
    bool notificationsOn = false;
    int baseActions = 2;

    // Start is called before the first frame update
    void Start() {
        turnText.text = $"Turn {World.turn}";
        // World.Init();
    }

    // Update is called once per frame
    void Update() {
        moneyText.text = $"Money: ${World.money:0,0}";

        if (actionText)
            actionText.text = $"Actions Remaining: {World.actionsRemaining}";
    }

    public void IncrementTurn() {
        World.turn++;
        turnText.text = $"Turn {World.turn}";
        World.actionsRemaining = baseActions;
    }

    public void ToggleSettings() {
        settingsOn = !settingsOn;

        if (settingsGroup)
            settingsGroup.SetActive(settingsOn);
    }

    public void ToggleInfo() {
        infoOn = !infoOn;

        if (infoGroup) {
            // infoGroup.gameObject.SetActive(infoOn);
            infoGroup.bRenderOnNextFrame = true;
        }
    }

    public void ToggleNotifications() {
        notificationsOn = !notificationsOn;

        if (notificationGroup)
            notificationGroup.SetActive(notificationsOn);
    }

    public void Exit() => GameManager.QuitGame();

    public void ChangeLevel(string level) => GameManager.Transition(level);

}
