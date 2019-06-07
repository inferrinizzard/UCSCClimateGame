using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GrassScript : MonoBehaviour
{
    // Reference to the tree prefab to spawn on click
    public TreeScript TreePrefab;

    // Reference to our stat controller for our UI widget
    public StatController sc;
    // Start is called before the first frame update
    void Start()
    {
        // For each tree in forest tree locations, add it to the scene and increase it's age by one
        foreach(KeyValuePair<Vector3, int> kvp in GlobalStatics.ForestTreeLocations)
        {
            TreeScript NewTree = Instantiate(TreePrefab, kvp.Key, transform.rotation) as TreeScript;
            NewTree.Age = kvp.Value;
            NewTree.GrowAYear();
        }

        // Empty out forest tree locations (we fill it back up when we leave the scene)
        GlobalStatics.ForestTreeLocations.Clear();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    // When the mouse is down, try to charge $25 and if successful spawn a tree
    private void OnMouseDown()
    {
        if (Input.GetKey("p") && GlobalStatics.CashMoney >= 25f)
        {
            GlobalStatics.CashMoney -= 25;
            sc.CashChange(-25f);

            Vector3 mouseLocation = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            mouseLocation.z = -.2f;

            TreeScript NewTree = Instantiate(TreePrefab, mouseLocation, transform.rotation) as TreeScript;
            NewTree.GrowAYear();
        }
    }

    // Populates the forest tree locations dictionary with all the trees in the scene
    // Call this when the back to main menu button is hit
    public void PopulateTreeDictionary()
    {
        GameObject[] trees = GameObject.FindGameObjectsWithTag("Tree");

        for(int i = 0; i < trees.Length; ++i)
        {
            TreeScript tree = trees[i].GetComponent<TreeScript>();
            GlobalStatics.ForestTreeLocations.Add(new KeyValuePair<Vector3, int> (trees[i].transform.position, tree.Age));
        }
    }
}
