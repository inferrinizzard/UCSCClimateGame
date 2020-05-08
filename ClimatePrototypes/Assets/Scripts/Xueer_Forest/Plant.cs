using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Plant : MonoBehaviour
{
    public PlantTree plantTree;
    public GameObject treeStage1Prefab;

    private Transform selectedAgent;
    private Vector3 selectedAgentPosition;
    private Vector3Int cellPosition;
    void Update()
    {
        if (plantTree.agentSelected != null && Input.GetButtonDown("Fire1"))
        {
            
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit2D hit = Physics2D.GetRayIntersection(ray,Mathf.Infinity);

            if (hit.collider != null && hit.collider.transform == gameObject.transform)
            {
                selectedAgentPosition = plantTree.agentSelected.transform.position;
          
                if (plantTree.canPlantTree)
                {
                    cellPosition = plantTree.tilemap.WorldToCell(selectedAgentPosition);
                    if (!plantTree.gridTreeInfo.ContainsKey(cellPosition))
                    {
                        // coroutine of 3 sec planting animation loop
                        plantTree.agentSelected.GetComponent<Animator>().SetInteger("animState", 3);
                        StartCoroutine("Shoveling");
                    }
                }
            }
        }
    }

    IEnumerator Shoveling()
    {
        yield return new WaitForSeconds(3f);
        plantTree.agentSelected.GetComponent<Animator>().SetInteger("animState", 0);
        GameObject go = Instantiate(treeStage1Prefab, selectedAgentPosition, transform.rotation);
        go.GetComponent<TreeGrowth>().enabled = true;
        plantTree.gridTreeInfo.Add(cellPosition, true); // add tree to data structure
        plantTree.gridTreePrefab.Add(cellPosition, go);
    }
}
