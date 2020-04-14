using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.UI;

public class RegionController : MonoBehaviour {
	[HideInInspector] public int visited = 0;
	public RegionIntro intro;
	[SerializeField] GameObject introPrefab = default;
	// timer logic?
	protected bool paused = false;
	protected bool updated = false;

	public void Intro() {
		if (visited++ > 0)
			return;
		Time.timeScale = 0;
		var introBlock = Instantiate(introPrefab); // could read different prefab from scriptable obj per visit // store func calls on scriptable obj?
		var introText = introBlock.GetComponentInChildren<Text>();
		var introButton = introBlock.GetComponentInChildren<Button>(true);
		introButton?.onClick.AddListener(new UnityEngine.Events.UnityAction(() => Time.timeScale = 1));
		StartCoroutine(UIController.ClickToAdvance(introText, intro.tutorial, introButton.gameObject));
	}

	protected void Pause(bool activatePrompt = true) {
		if (!paused) {
			paused = true;
			Time.timeScale = 0;
			UIController.Instance.SetPrompt(activatePrompt);
			updated = false;
		}
	}

	protected void TriggerUpdate(System.Action updateEBM) {
		if (!updated) {
			updateEBM();
			updated = true;
		}
	}
}
