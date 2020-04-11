using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LoadScene : MonoBehaviour
{
    public Transform agentTransform;

    public Transform levelEndTransform;

    // Update is called once per frame
    void Update()
    {
        if (agentTransform.position.x > levelEndTransform.position.x)
        {
            SceneManager.LoadScene("Xueer_Forest");
        }
    }
}
