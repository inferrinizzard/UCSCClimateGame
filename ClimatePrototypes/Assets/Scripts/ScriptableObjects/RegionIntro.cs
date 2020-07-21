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
			if (i == 0)
				return tutorial;
			if (i == 1)
				return secondVisit;
			if (i == 2)
				return secondVisit;
			return new string[0];
		}
	}
}
