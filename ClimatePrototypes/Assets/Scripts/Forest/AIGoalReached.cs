using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AIGoalReached : MonoBehaviour
{
    public GameObject myAgent;
    private void OnTriggerEnter2D(Collider2D other)
    {
        Debug.Log("trigger entered :  " + other.name);
        if (myAgent != null)
        {
            if (other.name == myAgent.name)
            {
                myAgent.GetComponent<Animator>().SetInteger("animState", 0);
            }
        }
        
    }
}
