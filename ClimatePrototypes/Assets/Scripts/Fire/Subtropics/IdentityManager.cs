using System.Collections;
using System.Collections.Generic;

using UnityEngine;
/// <summary>
/// Controls the identity of the cell
/// </summary>
public class IdentityManager : MonoBehaviour {
	public Identity id;
	public Moisture moisture;

	private int fireVariance = 0; // 0 for green, 1 for tree. keep track of the nature of the cell before fire
	public enum Identity { Fire, Green, Water, Tree }

	public int GetFireVariance() {
		return fireVariance;
	}
	public void SetFireVariance(int x) {
		fireVariance = x;
	}
	/// <summary>
	/// Controls the chance of it being ignited
	/// </summary>
	public enum Moisture { Moist, /* not likely that it could be ignited */ Normal, Dry }

	// Start is called before the first frame update
	void Start() {
		moisture = Moisture.Normal; // defalt moisture for all cells
	}

	// Update is called once per frame
	void Update() {
		IdentityCheck();
		//IdentityMutateToFire();
	}

	void IdentityCheck() {
		if (id is Identity.Fire) {
			gameObject.GetComponent<FireID>().enabled = true;
			gameObject.GetComponent<WaterID>().enabled = false;
			gameObject.GetComponent<GreenID>().enabled = false;
			gameObject.GetComponent<TreeID>().enabled = false;

		}
		if (id is Identity.Water) {
			gameObject.GetComponent<FireID>().enabled = false;
			gameObject.GetComponent<WaterID>().enabled = true;
			gameObject.GetComponent<GreenID>().enabled = false;
			gameObject.GetComponent<TreeID>().enabled = false;

		}
		if (id is Identity.Green) {
			gameObject.GetComponent<FireID>().enabled = false;
			gameObject.GetComponent<WaterID>().enabled = false;
			gameObject.GetComponent<GreenID>().enabled = true;
			gameObject.GetComponent<TreeID>().enabled = false;
		}
		if (id is Identity.Tree) {
			gameObject.GetComponent<FireID>().enabled = false;
			gameObject.GetComponent<WaterID>().enabled = false;
			gameObject.GetComponent<GreenID>().enabled = false;
			gameObject.GetComponent<TreeID>().enabled = true;
		}
	}

	private void OnMouseDown() {
		// if clicked on cell, add this to player path
		//Debug.Log("cell clicked");
		PlayerInteractions.addDestinationToPath(gameObject.transform);
	}
}
