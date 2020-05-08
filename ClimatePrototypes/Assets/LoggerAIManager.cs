using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Tilemaps;

public class LoggerAIManager : MonoBehaviour
{
    public GameObject loggerPrefab;

    //public bool newTreetoCut;
    public Tilemap tilemap;
    public Vector3Int spawnVector3Int;  // logger spawn location
    public List<Vector3Int> treeQueue = new List<Vector3Int>();
    
    // Update is called once per frame
    void Update()
    {
        if (treeQueue.Count > 0)
        {
            Vector3Int treeVector3Int = treeQueue.First();
            treeQueue.RemoveAt(0);
            Debug.Log("spawn logger" + treeQueue.Count);
            // spawn ai at right side of screen
            GameObject logger = Instantiate(loggerPrefab, tilemap.GetCellCenterWorld(spawnVector3Int), transform.rotation);
            logger.SetActive(true);
            logger.GetComponent<LoggerMovement>().GoToTree(tilemap.GetCellCenterWorld(treeVector3Int));
            //treeQueue.Clear();
        }
    }
}
