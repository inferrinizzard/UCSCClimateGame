using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public class Wind : MonoBehaviour {
	public Text windSpeedText;
	public Transform WindDirArrow;

	public enum WindDir { NE, NW, SE, SW }
	public WindDir dir = WindDir.NE;
	public int windSpeed = 10;

	void Start() {
		StartCoroutine(UpdateWindDirection());
	}

	void Update() {
		AdjustWindArrow();
	}

	void AdjustWindArrow() {
		//windSpeedText.text = $"{windSpeed} km/h";
		// arrow size
		float smooth = 5.0f;
		float arrowSize = 0.3f + (windSpeed - 5) / 2 * 0.1f;
		if (windSpeed == 0) {
			arrowSize = 0.3f;
		}

		Vector3 targetSize = new Vector3(arrowSize, arrowSize, 0);
		WindDirArrow.localScale = Vector3.Slerp(WindDirArrow.localScale, targetSize, Time.deltaTime * smooth);

		// arrow dir
		float tiltAngle = Array.IndexOf(new [] { WindDir.NE, WindDir.SE, WindDir.SW, WindDir.NW }, dir) * -90 + 135;
		Quaternion target = Quaternion.Euler(0, 0, tiltAngle);
		WindDirArrow.rotation = Quaternion.Slerp(WindDirArrow.rotation, target, Time.deltaTime * smooth);
	}

	IEnumerator UpdateWindDirection(float timer = 30) {
		yield return new WaitForSeconds(timer);
		dir = new [] { WindDir.NE, WindDir.NW, WindDir.SE, WindDir.SW }[Random.Range(0, 4)];
		windSpeed = Random.Range(5, 18);
		StartCoroutine(UpdateWindDirection());
	}
}
