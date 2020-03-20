using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.SceneManagement;

public class WorldRegion : MonoBehaviour {
	Transform parent, bg;
	// Start is called before the first frame update
	void Start() {
		parent = transform.parent;
		bg = parent.parent.GetChild(0);
	}

	// Update is called once per frame
	void Update() { }

	// void OnMouseOver()
	// {
	// 	for (int i = 0; i < parent.childCount; i++)        //sets all other regions to false(allows transparent bg to show)
	// 		if(i != transform.GetSiblingIndex())
	// 			parent.GetChild(i).gameObject.SetActive(false);
	// 	bg.gameObject.SetActive(false);     //sets solid bg to false
	// 	if(Input.GetMouseButtonDown(0) && World.actionsRemaining > 0)
	// 		if(MainMenuController.Scenes[name] != -1)
	// 		{
	// 			SceneManager.LoadScene(MainMenuController.Scenes[name]);
	// 			World.actionsRemaining--;
	// 		}
	// }

	void OnMouseExit() { //set solid bg active, everything else off
		for (int i = 0; i < parent.childCount; i++)
			parent.GetChild(i).gameObject.SetActive(true);
		bg.gameObject.SetActive(false);
	}
}
