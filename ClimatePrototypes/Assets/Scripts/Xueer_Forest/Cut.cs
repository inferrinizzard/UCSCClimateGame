using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cut : MonoBehaviour
{
    public PlantTree plantTree;


    private Transform selectedAgent;

    void Update()
    {
        if (plantTree.agentSelected != null && Input.GetButtonDown("Fire1"))
        {

            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit2D hit = Physics2D.GetRayIntersection(ray, Mathf.Infinity);

            if (hit.collider != null && hit.collider.transform == gameObject.transform)
            {
                Vector3 selectedAgentPosition = plantTree.agentSelected.transform.position;
                Vector3Int cellPosition = plantTree.tilemap.WorldToCell(selectedAgentPosition);

                if (plantTree.gridTreeInfo.ContainsKey(cellPosition))
                {
                    plantTree.gridTreePrefab[cellPosition].GetComponent<TreeGrowth>().treeStage = 6;
                    plantTree.gridTreePrefab[cellPosition].GetComponent<TreeGrowth>().UpdateTreeVFX(6);
                }
            }
        }
    }
}
