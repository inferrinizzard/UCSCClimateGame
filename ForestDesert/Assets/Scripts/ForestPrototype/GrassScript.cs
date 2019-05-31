using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GrassScript : MonoBehaviour
{
    public TreeScript TreePrefab;
    public StatController sc;
    // Start is called before the first frame update
    void Start()
    {
        foreach(KeyValuePair<Vector3, int> kvp in GlobalStatics.ForestTreeLocations)
        {
            TreeScript NewTree = Instantiate(TreePrefab, kvp.Key, transform.rotation) as TreeScript;
            NewTree.Age = kvp.Value;
            NewTree.GrowAYear();
        }

        GlobalStatics.ForestTreeLocations.Clear();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

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
