using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class FadeFire : MonoBehaviour {
	public float FadeRate;
	private Image image;
	private float targetAlpha;
	// Use this for initialization
	void Start() {
		this.image = this.GetComponent<Image>();
		this.targetAlpha = this.image.color.a;
	}

	// Update is called once per frame
	void Update() {
		Color curColor = this.image.color;
		float alphaDiff = Mathf.Abs(curColor.a - this.targetAlpha);
		if (alphaDiff > 0.0001f) {
			curColor.a = Mathf.Lerp(curColor.a, targetAlpha, this.FadeRate * Time.deltaTime);
			this.image.color = curColor;
		}
	}

	public void FadeOut() {
		this.targetAlpha = 0.0f;
	}

	public void FadeIn() {
		this.targetAlpha = this.image.color.a;
	}
}
