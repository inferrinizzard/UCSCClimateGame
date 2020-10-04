using System.Collections;
using System.Collections.Generic;

using UnityEngine;

public class CameraFade : MonoBehaviour {
	Material fadeMat;
	void Start() => fadeMat = new Material(Shader.Find("Screen/Fade"));

	public IEnumerator FadeIn(float time) {
		for (var (start, step) = (Time.time, 0f); step < time; step = Time.time - start) {
			yield return null;
			fadeMat.SetFloat("_Alpha", 1 - step / time); // slow
		}
	}

	void OnRenderImage(RenderTexture src, RenderTexture dest) => Graphics.Blit(src, dest, fadeMat);
}
