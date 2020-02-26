using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LowCloud : MonoBehaviour {
	private float brightnessThreshold = .4f;
	
	Vector2 screenMin;
	private Vector2 screenMax;
	public float sideForce = 3f;
	Rigidbody2D rb;
	
	public Sprite brightCloud;
	public Sprite darkCloud;
	
	void Start()
	{
		screenMin = Camera.main.ViewportToWorldPoint(Vector2.zero);
		screenMax = Camera.main.ViewportToWorldPoint(Vector2.one);
		rb = GetComponent<Rigidbody2D>();

		Vector2 force = new Vector2(sideForce, 0);
		rb.velocity = force;
		
		if (Random.value > brightnessThreshold) {
			
			gameObject.GetComponent<SpriteRenderer>().sprite = brightCloud;
		}
		else
		{
			GetComponent<Collider2D>().enabled = false;
			gameObject.GetComponent<SpriteRenderer>().sprite = darkCloud;
		}
	}

	//void OnTriggerEnter2D(Collider2D other) {}
	//void OnTriggerExit2D(Collider2D other) => GetComponent<Collider2D>().enabled = true;
	
	void Update() {

		if (transform.position.x < screenMin.x || transform.position.x > screenMax.x)
			Destroy(gameObject);
	}
}
