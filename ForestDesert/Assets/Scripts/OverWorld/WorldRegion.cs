using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class WorldRegion : MonoBehaviour
{
    Transform parent, bg;
    // Start is called before the first frame update
    void Start()
    {
        parent = transform.parent;
        bg = parent.parent.GetChild(0);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void OnMouseOver(){
        // Debug.Log(name);
        for(int i=0;i<parent.childCount;i++)
            if(i!=transform.GetSiblingIndex())
                parent.GetChild(i).gameObject.SetActive(false);
        bg.gameObject.SetActive(false);
        if(Input.GetMouseButtonDown(0))
            if(MainMenuController.Scenes[name]!=-1)
                SceneManager.LoadScene(MainMenuController.Scenes[name]);
    }

    void OnMouseExit(){
        for(int i=0;i<parent.childCount;i++)
            parent.GetChild(i).gameObject.SetActive(true);
        bg.gameObject.SetActive(false);
    }
}
