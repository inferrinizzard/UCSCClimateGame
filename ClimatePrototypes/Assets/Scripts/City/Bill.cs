using System.Collections;
using System.Collections.Generic;
using System.Linq;

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

	public void SetBill(CityScript.BillData.BillHalf currentBill) {
		Print(currentBill.title, currentBill.body);
		ArrangeIcons(currentBill.effects);
	}

	void ArrangeIcons(Dictionary<string, float> effects) {
		foreach (Transform child in iconWrapper.transform)
			child.gameObject.SetActive(effects.ContainsKey(child.name));
	}

	void Print(string titleText, string bodyText) {
		StartCoroutine(UIController.Typewriter(title, titleText, speed));
		StartCoroutine(UIController.Typewriter(body, bodyText, speed));
	}
}
