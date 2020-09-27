using System.Collections;
using System.Collections.Generic;
using System.Linq;

using UnityEngine;
/// <summary>
/// Controls the identity of the cell
/// </summary>
public class IdentityManager : MonoBehaviour {
	Identity _id = Identity.Green;
	public Identity id { get => _id; set { _id = value; UpdateTile(); } }
	public Moisture moisture = Moisture.Normal;
	public int fireVariance { get; set; } = 0; // 0 for green, 1 for tree. keep track of the nature of the cell before fire
	public enum Identity { Fire, Water, Green, Tree }
	/// <summary> Controls the chance of it being ignited </summary>
	public enum Moisture { Moist, /* not likely that it could be ignited */ Normal, Dry }

	void UpdateTile() => GetComponents<Tile>().ToList().ForEach((o, i) => o.enabled = i == (int) id);

	void OnMouseDown() {
		// if clicked on cell, add this to player path
		//Debug.Log("cell clicked");
		PlayerInteractions.addDestinationToPath(gameObject.transform);
	}
}

public abstract class Tile : MonoBehaviour {
	protected IdentityManager idManager;
	protected SpriteRenderer sr;
	void Awake() {
		sr = GetComponent<SpriteRenderer>();
		idManager = GetComponent<IdentityManager>();
	}

	void OnEnable() => UpdateTile();

	protected abstract void UpdateTile();
}
