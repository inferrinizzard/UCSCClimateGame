using System.Collections;
using System.Collections.Generic;

using UnityEngine;

public class DayNightCycle : MonoBehaviour {
	private SpriteRenderer daySR, nightSR;
	public bool isDayTime = false;
	private float dayDuration = 10f;
	System.Func<float, float, float> fadeIn, fadeOut;
	[SerializeField] float transitionTime = 1.5f;

	void Start() {
		var srs = GetComponentsInChildren<SpriteRenderer>();
		(daySR, nightSR) = (srs[0], srs[1]);

		// init fade funcs
		fadeIn = (float step, float dur) => EaseMethods.CubicEaseOut(step, 0, 1, dur);
		fadeOut = (float step, float dur) => EaseMethods.CubicEaseIn(dur - step, 0, 1, dur);

		StartCoroutine(ChangeDayNight(isDayTime, true));
	}

	/// <summary> Controls day-night cycle. </summary>
	IEnumerator ChangeDayNight(bool isDay, bool initial = false) {
		isDayTime = isDay;
		float timer = initial ? 2.5f : dayDuration;
		while ((timer -= Time.deltaTime) > dayDuration - transitionTime) {
			yield return null;
			float step = timer - (dayDuration - transitionTime);
			daySR.color = new Color(1, 1, 1, isDayTime ? fadeOut(step, transitionTime) : fadeIn(step, transitionTime));
			nightSR.color = new Color(1, 1, 1, isDayTime ? fadeIn(step, transitionTime) : fadeOut(step, transitionTime));
		}
		yield return new WaitForSeconds(initial ? timer : dayDuration - transitionTime);
		StartCoroutine(ChangeDayNight(!isDay));
	}
}
