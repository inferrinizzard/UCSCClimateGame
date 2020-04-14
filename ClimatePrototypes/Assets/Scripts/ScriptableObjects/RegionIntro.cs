using System.Collections;
using System.Collections.Generic;

using UnityEngine;

[CreateAssetMenu(fileName = "RegionIntro", menuName = "ScriptableObjects/RegionIntro", order = 1)]
public class RegionIntro : ScriptableObject {
	public string[] tutorial;
	public GameObject prefab;
}
