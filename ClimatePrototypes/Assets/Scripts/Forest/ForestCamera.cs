using System.Collections;
using System.Collections.Generic;

using UnityEngine;
public class ForestCamera : MonoBehaviour {
	[SerializeField] SpriteRenderer station = default, forest = default, factory = default;
	List<GameObject> enableTargets = new List<GameObject>();

	public static float GetScreenToWorldHeight { get => Camera.main.ViewportToWorldPoint(new Vector2(1, 1)).y * 2; }
	public static float GetScreenToWorldWidth { get => Camera.main.ViewportToWorldPoint(new Vector2(1, 1)).x * 2; }

	void Start() {
		foreach (var s in new [] { station, forest, factory })
			s.transform.localScale = Vector3.one * GetScreenToWorldHeight / s.sprite.bounds.size.y;
		station.transform.position = new Vector3(forest.sprite.bounds.min.x * forest.transform.localScale.x - station.sprite.bounds.extents.x * station.transform.localScale.x, 0, 0);
		factory.transform.position = new Vector3(forest.sprite.bounds.max.x * forest.transform.localScale.x + factory.sprite.bounds.extents.x * factory.transform.localScale.x, 0, 0);
	}

	void Update() {
		// 	if (Input.mousePosition.x > Screen.width * .9) {
		// 		if (transform.position.x < factory.transform.position.x) //TODO: limit height
		// 			transform.Translate(Time.deltaTime * speed, 0, 0); //TODO: could ease speed by distance to target
		// 	} else if (Input.mousePosition.x < Screen.width * .1) {
		// 		if (transform.position.x > station.transform.position.x) //TODO: limit height
		// 			transform.Translate(-Time.deltaTime * speed, 0, 0); //TODO: could ease speed by distance to target
		// 	}
	}

	IEnumerator SlideCamera(Vector2 target, Ease ease, float time = 1) {
		Vector2 startPos = transform.position;
		for (var(start, step) = (Time.time, 0f); step <= time; step = Time.time - start) {
			yield return null;
			transform.position = (Vector3) (ease.Invoke(step / time, 0, 1, 1) * (target - startPos) + startPos) + Vector3.forward * transform.position.z;
		}
		foreach (var obj in enableTargets)
			obj.SetActive(true);
		enableTargets.Clear();
	}

	public void AddEnableTarget(GameObject obj) => enableTargets.Add(obj);
	public void ToForest(bool urgent = false) => StartCoroutine(SlideCamera(forest.transform.position, urgent ? EaseMethods.QuintEaseInOut : EaseMethods.QuartEaseInOut));
	public void ToFactory(bool urgent = false) => StartCoroutine(SlideCamera(factory.transform.position, urgent ? EaseMethods.QuintEaseInOut : EaseMethods.QuartEaseInOut));
	public void ToFacility(bool urgent = false) => StartCoroutine(SlideCamera(station.transform.position, urgent ? EaseMethods.QuintEaseInOut : EaseMethods.QuartEaseInOut));

}
