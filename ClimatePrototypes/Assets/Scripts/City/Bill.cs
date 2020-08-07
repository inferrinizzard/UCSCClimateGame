using UnityEngine;
using UnityEngine.UI;

public class Bill : MonoBehaviour {
	[SerializeField] Text title = default, body = default;
	float iconSize;
	[SerializeField] GameObject iconWrapper = default;
	RectTransform iconAnchor;
	[HideInInspector] public float speed;

	void Start() {
		iconAnchor = iconWrapper.transform.GetChild(0) as RectTransform;
		iconSize = iconAnchor.rect.width;
	}

	public void Print(string titleText, string bodyText) {
		StartCoroutine(UIController.Typewriter(title, titleText, speed));
		StartCoroutine(UIController.Typewriter(body, bodyText, speed));
	}
}
