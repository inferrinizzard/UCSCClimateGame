using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LoadSceneForest : MonoBehaviour
{
    public Transform backColliderTransform;

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit2D hit = Physics2D.GetRayIntersection(ray,Mathf.Infinity);
            
            if (hit.collider != null && hit.collider.transform == backColliderTransform)
            {
                SceneManager.LoadScene("ForestMainScreen");
            }
        }
    }
}
