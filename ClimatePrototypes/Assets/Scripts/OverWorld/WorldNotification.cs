using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class WorldNotification : MonoBehaviour {
	public Dictionary<Variables, Regions[]> regionVariables = new Dictionary<Variables, Regions[]>();

	public enum Variables {
		opinion,
		money,
		co2,
		land
	}

	public enum Regions {
		city,
		tropics,
		arctic,
		forest
	}

	// Start is called before the first frame update
	void Start() {
		regionVariables.Add(Variables.opinion, new Regions[] { Regions.city });
		regionVariables.Add(Variables.money, new Regions[] { Regions.city });
		regionVariables.Add(Variables.co2, new Regions[] { Regions.city });
		regionVariables.Add(Variables.land, new Regions[] { Regions.city });

		notifyWorld(Variables.opinion); // test
	}

	// called by updateXXX functions in World script
	public void notifyWorld(Variables value) {
		// Debug.Log(value + " updated, popping badges in following regions:");
		foreach (var region in regionVariables[value]) {
			// Debug.Log(region);
			//  pop corresponding bagdges
		}

	}
}
