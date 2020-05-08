using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class LoggerAIManager : MonoBehaviour
{
    public GameObject loggerPrefab;

    public bool newTreetoCut;
    public Tilemap tilemap;
    public Vector3Int spawnVector3Int;

    public Queue<Vector3Int> treeList = new Queue<Vector3Int>();
    // Start is called before the first frame update
    void Start()
    {
        treeList = new Queue<Vector3Int>();
    }

    // Update is called once per frame
    void Update()
    {
        if (newTreetoCut)
        {
            // spawn ai at right side of screen
            Instantiate(loggerPrefab, tilemap.GetCellCenterWorld(spawnVector3Int), transform.rotation);
            newTreetoCut = false;
        }
    }
}
