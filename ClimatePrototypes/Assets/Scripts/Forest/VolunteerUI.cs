using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.UI;

public class VolunteerUI : MonoBehaviour {
	[SerializeField] Sprite[] bubbles = default;
	[SerializeField] Sprite /*active = default,*/ vacant = default;
	[SerializeField] GameObject selector = default, bubble = default;

	bool selected { get => (ForestController.Instance as ForestController).selected == this; }
	Animator animator;

	void Start() {
		animator = GetComponent<Animator>();
	}

	public void Deactivate() {
		GetComponent<Image>().sprite = vacant;
		GetComponent<Button>().enabled = false;
		bubble.SetActive(false);
	}

	public void SelectUI() {
		(ForestController.Instance as ForestController).selected = selected != this ? this : null;
		selector.SetActive(selected);
	}

	public void Reset() {
		GetComponent<Button>().enabled = true;
		selector.SetActive(selected);
		gameObject.SetActive(true);
		bubble.GetComponent<Image>().sprite = bubbles[0];
	}

	public void AssignBubble(UnityEngine.Events.UnityAction<Volunteer> action) {
		selector.SetActive(false);
		GetComponent<Button>().enabled = false;
		bubble.GetComponent<Image>().sprite = bubbles[VolunteerActions.GetBubble(action)];
	}
}
