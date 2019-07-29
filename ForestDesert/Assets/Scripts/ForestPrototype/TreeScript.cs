using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TreeScript : MonoBehaviour
{
	// How long (seconds) it takes for a tree to fully mature
	public float GrowthRate = 8f;
	// An array of the sprites to choose from when displaying the tree
	public Sprite[] Sprites;

	// A reference to the stat controller
	private StatController sc;
	// Whether or not a tree is fully grown
	private bool bGrown = false;
	// Keeps track of how long the tree has been growing. Increments with time
	private float GrowthTime = 0f;
	// Reference to the DesertShifter object
	private DesertShifter ds;

	public ParticleSystem ps;

	// Start is called before the first frame update
	void Start()
	{
		// Set our sprite to a random tree sprite
		SpriteRenderer sr = GetComponent<SpriteRenderer>();
		sr.sprite = Sprites[Mathf.FloorToInt(Random.Range(0, Sprites.Length))];

		// Find the desert shifter
		ds = GameObject.FindGameObjectWithTag("Water").GetComponent<DesertShifter>();

		sc = (StatController)Object.FindObjectOfType(typeof(StatController));
	}

	// Update is called once per frame
	void Update()
	{
		if (!bGrown)
		{
			// Increment GrowthTime. Keep it between 0 and GrowthRate
			GrowthTime = Mathf.Clamp(GrowthTime + Time.deltaTime, 0f, GrowthRate);
			float currScale = transform.localScale.x;

			// Lerp the current scale GrowthTime/GrowthRate percent. 
			// 4s / 8s would be 50% grown, so make it 50% as big as a grown tree
			currScale = Mathf.Lerp(.18f, .3f, GrowthTime / GrowthRate);
			transform.localScale = new Vector3(currScale, currScale, currScale);

			// The moment the tree matures see if it's close to the desert line and then shift it
			if (GrowthTime >= GrowthRate)
			{
				bGrown = true;
				ps.Play();

				if (transform.position.x > ds.transform.position.x - 5f)
				{
					ds.ShiftRight(.5f);
				}
			}
		}
	}

	private void OnTriggerEnter2D(Collider2D other)
	{
		if (other.tag == "Cloud")
		{
			Destroy(this.gameObject);
		}
	}

	private void OnMouseOver()
	{
		if (Input.GetMouseButtonDown(1) && bGrown)
		{
			GlobalStatics.cashMoney += 45;
			sc.CashChange(45);
			Destroy(this.gameObject);
		}
	}
}
