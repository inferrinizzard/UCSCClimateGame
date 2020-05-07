using System.Collections;
using System.Collections.Generic;
using Pathfinding;
using UnityEngine;

public class LoggerMovement : MonoBehaviour
{
    private Transform currentAgentGoal;
    public LoggerAIManager loggerAI;
    public GameObject goalPrefab;
    private Vector3Int cutLocation;  // which tree am I going to cut
    // Start is called before the first frame update
    void Start()
    { 
        currentAgentGoal = gameObject.GetComponent<AIDestinationSetter>().target;
        //currentAgentGoal.position = new Vector3(5.5, 0, 0);
        //GameObject logGoal = Instantiate(goalPrefab, cutLocation, transform.rotation);
        //GetComponent<AIDestinationSetter>().target = logGoal.transform;
        
        // when spawned find a tree to log
        if (loggerAI.treeList.Count > 0)
        {
            cutLocation = loggerAI.treeList.Dequeue();
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (cutLocation != null && currentAgentGoal != null)
        {
            
            currentAgentGoal.position = cutLocation;
        
            GetComponent<Animator>().SetInteger("animState", 2);
        }
        
    }
}
