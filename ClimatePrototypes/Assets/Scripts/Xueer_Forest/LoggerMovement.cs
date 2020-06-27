using System.Collections;
using System.Collections.Generic;
using Pathfinding;
using UnityEngine;
using UnityEngine.Tilemaps;

public class LoggerMovement : MonoBehaviour
{
    private Transform currentAgentGoal;
    public GameObject goalPrefab;
    private Vector3Int cutLocation;  // which tree am I going to cut
    private Transform originTransform;
    public Tilemap tilemap;

    public PlantTree plantTree;

    private bool treeCut;
    void Start()
    {
        //originTransform = gameObject.GetComponent<Transform>();a
        //currentAgentGoal = originTransform;
        //currentAgentGoal = gameObject.GetComponent<AIDestinationSetter>().target;
        //currentAgentGoal.position = new Vector3(5.5, 0, 0);
        //GameObject logGoal = Instantiate(goalPrefab, cutLocation, transform.rotation);
        //GetComponent<AIDestinationSetter>().target = logGoal.transform;
        
        // when spawned find a tree to log
        /*if (loggerAI.treeQueue.Count > 0)
        {
            cutLocation = loggerAI.treeQueue.Dequeue();
        }*/
    }

    public void GoToTree(Vector3 tree)
    {
        GameObject go = Instantiate(goalPrefab, tree, transform.rotation);
        GetComponent<AIDestinationSetter>().target = go.transform;
        GetComponent<Animator>().SetInteger("animState", 2);
    }

    // Update is called once per frame
    void Update()
    {
        if (GetComponent<AIDestinationSetter>().target != null && !treeCut)
        {
            // if reached goal cutting tree animation
            if (tilemap.WorldToCell(transform.position) == tilemap.WorldToCell(GetComponent<AIDestinationSetter>().target.position))
            {
                GetComponent<Animator>().SetInteger("animState", 4);
                StartCoroutine("CuttingTree");
            }
        }
        

    }
    
    IEnumerator CuttingTree()
    {
        yield return new WaitForSeconds(5f);
        GetComponent<Animator>().SetInteger("animState", 0);
        treeCut = true;
        plantTree.CutTreeAt(tilemap.WorldToCell(transform.position));
        
        // destroy it self
        Destroy(gameObject);
    }
}
