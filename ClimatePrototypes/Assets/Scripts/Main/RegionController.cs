using System.Collections;
using System.Collections.Generic;

using UnityEngine;

public class RegionController : MonoBehaviour {
	// timer logic?

	protected void Pause( /*string scene/text */ ) {
		Time.timeScale = 0;
		UIController.Instance.SetPrompt(true);
	}
}
