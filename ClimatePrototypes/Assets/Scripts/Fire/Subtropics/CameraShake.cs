using System.Collections;
using System.Collections.Generic;

using UnityEngine;

public class CameraShake : MonoBehaviour {
	public IEnumerator Shake(float duration, float magnitude) {
		Vector3 originalPos = transform.localPosition;
		for (float elapsed = 0.0f; elapsed < duration; elapsed += Time.deltaTime) {
			float x = Random.Range(-1f, 1f) * magnitude;
			float y = Random.Range(-1f, 1f) * magnitude;

			transform.localPosition = new Vector3(x + originalPos.x, y + originalPos.y, originalPos.z);
			yield return null;
		}
		transform.localPosition = originalPos;
	}
}
