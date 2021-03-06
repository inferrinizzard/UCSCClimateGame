using System.Collections;
using System.Collections.Generic;

using UnityEngine;

public class Buffer : MonoBehaviour {
	[HideInInspector] public int health = 5;
	/// <summary> ice sprites in order </summary>
	[SerializeField] Sprite[] summerSprite = new Sprite[6];
	[SerializeField] Sprite[] winterSprite = new Sprite[6];
	SpriteRenderer sr;

	void Start() {
		health = summerSprite.Length - 1;
		sr = GetComponent<SpriteRenderer>();
		AssignSprite();
	}

	/// <summary> Updates Buffer sprite </summary>
	public void AssignSprite(int i = -1) => sr.sprite = ArcticController.Instance.summer ? summerSprite[i > 0 ? i : health] : winterSprite[i > 0 ? i : health];

	/// <summary> handles solar radiation </summary>
	void OnTriggerEnter2D(Collider2D collision) {
		if (collision.transform.TryGetComponent(out RadiationBall R)) { // check if solar radiation
			AudioManager.Instance.Play("SFX_Ice_Break"); // TODO: move audio name to static class
			if (health > 0)
				AssignSprite(--health);
			else
				Destroy(GetComponent<Collider2D>()); // turn off collision when health empty
			Destroy(collision.gameObject);
		}
	}
}
