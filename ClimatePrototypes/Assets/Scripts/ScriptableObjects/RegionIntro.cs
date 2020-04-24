using System.Collections;
using System.Collections.Generic;

using UnityEngine;

[CreateAssetMenu(fileName = "RegionIntro", menuName = "ScriptableObjects/RegionIntro", order = 1)]
public class RegionIntro : ScriptableObject {
	public string[] tutorial;
	public string[] secondVisit;
	public string[] thirdVisit;

	public string[] this [int i] {
		get {
			string[] output;
			try {
				output = new List<string[]>() { tutorial, secondVisit, thirdVisit }[i];
			} catch {
				output = new string[0];
			}
			return output;
		}
	}
}
