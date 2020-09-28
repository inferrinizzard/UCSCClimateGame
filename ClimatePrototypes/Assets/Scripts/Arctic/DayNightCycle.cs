using System.Collections;
using System.Collections.Generic;
using System.Linq;

using UnityEngine;

public class DayNightCycle : MonoBehaviour {
	SpriteRenderer daySR, nightSR;
	public bool isDayTime = false;
	float dayDuration = 20f;
	System.Func<float, float, float> fadeIn, fadeOut;
	[SerializeField] float transitionTime = 1.5f;

	void Start() {
		var srs = GetComponentsInChildren<SpriteRenderer>();
		(daySR, nightSR) = (srs[0], srs[1]);

		// init fade funcs
		fadeIn = (float step, float dur) => EaseMethods.CubicEaseOut(step, 0, 1, dur);
		fadeOut = (float step, float dur) => EaseMethods.CubicEaseIn(dur - step, 0, 1, dur);

		StartCoroutine(ChangeDayNight(isDayTime));
	}

	/// <summary> Controls day-night cycle. </summary>
	IEnumerator ChangeDayNight(bool isDay, float duration = .5f) {
		isDayTime = isDay;
		float timer = duration;
		while ((timer -= Time.deltaTime) > dayDuration - transitionTime) {
			yield return null;
			float step = timer - (dayDuration - transitionTime);
			daySR.color = new Color(1, 1, 1, isDayTime ? fadeOut(step, transitionTime) : fadeIn(step, transitionTime));
			nightSR.color = new Color(1, 1, 1, isDayTime ? fadeIn(step, transitionTime) : fadeOut(step, transitionTime));
		}
		ArcticController.Instance.buffers.ToList().ForEach(b => b.AssignSprite());
		yield return new WaitForSeconds(duration);
		StartCoroutine(ChangeDayNight(!isDay, dayDuration));
	}
}
