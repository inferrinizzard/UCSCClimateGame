using System.Collections;
using System.Collections.Generic;

using UnityEngine;

public class OverworldController : MonoBehaviour {
	[SerializeField] GameObject worldWrapper = default;
	[SerializeField] Transform moon = default;
	Transform world;
	[SerializeField] SpriteRenderer bg = default;
	[HideInInspector] public Material fadeMat;

	void Start() {
		fadeMat = new Material(Shader.Find("Screen/Fade"));
		world = worldWrapper.transform.GetChild(0);

		StartCoroutine(RotateMoon());
	}

	IEnumerator RotateMoon() {
		float moonDist = (world.position - moon.position).magnitude;
		float alpha = Vector2.Angle(Vector2.right, (Vector2) (moon.position - world.position)) * Mathf.Deg2Rad;
		SpriteRenderer moonSprite = moon.GetComponent<SpriteRenderer>();
		UnityEngine.SceneManagement.Scene overworldScene = UnityEngine.SceneManagement.SceneManager.GetActiveScene();

		while (overworldScene.isLoaded) {
			yield return null;
			float step = Time.time / 2;
			float x = moonDist * Mathf.Cos(step) * Mathf.Cos(alpha) - moonDist / 2 * Mathf.Sin(step) * Mathf.Sin(alpha);
			float y = moonDist * Mathf.Cos(step) * Mathf.Sin(alpha) + moonDist / 2 * Mathf.Sin(step) * Mathf.Cos(alpha);

			moon.transform.position = new Vector2(x, y) + (Vector2) world.position;
			moon.transform.eulerAngles = Vector3.forward * Mathf.Sin(step) * Mathf.Rad2Deg;
			moonSprite.sortingOrder = Mathf.Sin(step) > 0 ? 0 : 2;
		}
	}

	/// <summary> used to load world in from title screen </summary>
	public void SendToBottom() {
		Camera.main.transform.position = Vector3.forward * -10;
		bg.transform.position = new Vector3(bg.transform.position.x, -Camera.main.ViewportToWorldPoint(Vector2.zero).y - bg.bounds.extents.y, bg.transform.position.z); // TODO: global variable class
		Camera.main.transform.position = new Vector3(0, bg.bounds.min.y - Camera.main.ViewportToWorldPoint(Vector2.zero).y, -10); // TODO: global variable class
	}

	public void ClearWorld() {
		foreach (SpriteRenderer sr in worldWrapper.GetComponentsInChildren<SpriteRenderer>())
			sr.color = new Color(sr.color.r, sr.color.g, sr.color.b, 0);
	}

	public IEnumerator EnterWorld(float time = 1) {
		ClearWorld();
		SpriteRenderer[] sprites = worldWrapper.GetComponentsInChildren<SpriteRenderer>();
		for (var (start, step) = (Time.time, 0f); step < time; step = Time.time - start) {
			yield return null;
			foreach (var sr in sprites)
				sr.color = new Color(sr.color.r, sr.color.g, sr.color.b, step);
		}
	}

	void OnRenderImage(RenderTexture src, RenderTexture dest) {
		Graphics.Blit(src, dest, fadeMat);
	}
}
