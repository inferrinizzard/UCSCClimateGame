using UnityEngine;

public class ForestCamera : MonoBehaviour {
	[SerializeField] SpriteRenderer station = default, forest = default, factory = default;
	[SerializeField] float speed = 1;

	public static float GetScreenToWorldHeight { get => Camera.main.ViewportToWorldPoint(new Vector2(1, 1)).y * 2; }
	public static float GetScreenToWorldWidth { get => Camera.main.ViewportToWorldPoint(new Vector2(1, 1)).x * 2; }

	void Start() {
		foreach (var s in new [] { station, forest, factory })
			s.transform.localScale = Vector3.one * GetScreenToWorldHeight / s.sprite.bounds.size.y;
		station.transform.position = new Vector3(forest.sprite.bounds.min.x * 2, 0, 0) * forest.transform.localScale.x;
		factory.transform.position = new Vector3(forest.sprite.bounds.max.x * 2, 0, 0) * forest.transform.localScale.x;
	}

	void Update() {
		if (Input.mousePosition.x > Screen.width * .9) {
			if (transform.position.x < factory.transform.position.x) //TODO: limit height
				transform.Translate(Time.deltaTime * speed, 0, 0); //TODO: could ease speed by distance to target
		} else if (Input.mousePosition.x < Screen.width * .1) {
			if (transform.position.x > station.transform.position.x) //TODO: limit height
				transform.Translate(-Time.deltaTime * speed, 0, 0); //TODO: could ease speed by distance to target
		}
	}
}
